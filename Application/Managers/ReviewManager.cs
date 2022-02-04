﻿using System;
using System.Net;
using System.Threading.Tasks;
using Application.Errors;
using Application.ServiceInterfaces;
using AutoMapper;
using Domain.Entities;
using Models.Activity;

namespace Application.Managers
{
    public class ReviewManager : IReviewManager
    {

        private readonly IActivityReviewService _activityReviewService;
        private readonly IUserLevelingService _userLevelingService;
        private readonly IMapper _mapper;
        private readonly IActivityService _activityService;

        public ReviewManager(IActivityReviewService activityReviewService, IUserLevelingService userLevelingService, IMapper mapper, IActivityService activityService)
        {
            _activityReviewService = activityReviewService;
            _userLevelingService = userLevelingService;
            _mapper = mapper;
            _activityService = activityService;
        }

        public async Task ReviewActivityAsync(ActivityReview activityReview)
        {
            if (!await _userLevelingService.ReviewerExistsAsync(activityReview.UserId))
            {
                throw new RestException(HttpStatusCode.BadRequest, new { ActivityReview = "Greška, nepostojeći korisnik." });
            }

            var activity = _mapper.Map<UserReview>(activityReview);

            var activityCreatorId = (await _activityService.GetActivityUserIdByActivityId(activityReview.ActivityId)).User.Id;

            if (activityReview.UserId == activityCreatorId)
            {
                throw new RestException(HttpStatusCode.BadRequest, new { ActivityReview = "Greška, ne možete oceniti svoju aktivnost." });
            }

            var xpRewardToYield = await _userLevelingService.GetXpRewardYieldByReviewAsync(activity);

            var existingReview = await _activityReviewService.GetUserReviewByActivityAndUserId(activityReview.ActivityId, activityReview.UserId);

            if (existingReview == null)
            {
                await _activityReviewService.AddReviewActivityAsync(activityReview);

                await _userLevelingService.UpdateUserXpAsync(xpRewardToYield, activityCreatorId);

                return;
            }

            var existingXpReward = await _userLevelingService.GetXpRewardYieldByReviewAsync(existingReview);

            if (existingXpReward == xpRewardToYield)
            {
                return;
            }

            await _activityReviewService.UpdateReviewActivityAsync(activityReview);

            var difference = CalculateAmountToChange(xpRewardToYield, existingXpReward);

            await _userLevelingService.UpdateUserXpAsync(difference, activityCreatorId);
        }

        private int CalculateAmountToChange(int newValue, int oldValue)
        {
            return newValue > oldValue ? Math.Abs(newValue - oldValue) : -Math.Abs(newValue - oldValue);
        }
    }
}
