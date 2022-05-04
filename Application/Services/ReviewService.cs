using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces.Security;
using Application.Models.Activity;
using AutoMapper;
using DAL;
using Domain;
using LanguageExt;

namespace Application.ServiceInterfaces
{
    public class ReviewService : IReviewService
    {

        private readonly IUnitOfWork _uow;
        private readonly IUserAccessor _userAccessor;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork uow, IUserAccessor userAccessor, IMapper mapper)
        {
            _uow = uow;
            _userAccessor = userAccessor;
            _mapper = mapper;
        }

        public async Task<IList<UserReviewedActivity>> GetOwnerReviewsAsync()
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();
            var userReviews = await _uow.UserReviews.GetUserReviewsAsync(userId);

            return _mapper.Map<List<UserReviewedActivity>>(userReviews);
        }

        public async Task<Either<RestError, Unit>> ReviewActivityAsync(ActivityReview activityReview)
        {
            //TO DO: improve by sending activitycreatorid in request
            var reviewerId = _userAccessor.GetUserIdFromAccessToken();
            var creator = (await _uow.Activities.GetAsync(activityReview.ActivityId)).User;

            if (reviewerId == creator.Id)
                return new BadRequest("Ne možete oceniti svoju aktivnost.");

            var xpRewardToYield = await _uow.ActivityReviewXps.GetXpRewardAsync(activityReview.ActivityTypeId, activityReview.ReviewTypeId);

            var creatorSkill = await _uow.Skills.GetSkillAsync(creator.Id, activityReview.ActivityTypeId);

            var xpMultiplier = creatorSkill != null && creatorSkill.IsInSecondTree() ? await _uow.SkillXpBonuses.GetSkillMultiplierAsync(creatorSkill) : 1;

            var xpRewardValue = xpRewardToYield.Xp * xpMultiplier;

            var existingReview = await _uow.UserReviews.GetUserReviewAsync(activityReview.ActivityId, reviewerId);

            if (existingReview == null)
            {
                var review = _mapper.Map<UserReview>(activityReview);
                review.UserId = reviewerId;
                _uow.UserReviews.Add(review);

                creator.CurrentXp += xpRewardValue;

                await _uow.CompleteAsync();

                return Unit.Default;
            }

            var existingXpReward = await _uow.ActivityReviewXps.GetXpRewardAsync(existingReview.Activity.ActivityTypeId, existingReview.ReviewTypeId);
            var existingXpRewardValue = existingXpReward.Xp * xpMultiplier;

            if (existingXpRewardValue == xpRewardValue)
                return Unit.Default;

            existingReview.ReviewTypeId = activityReview.ReviewTypeId;

            var difference = xpRewardValue > existingXpRewardValue ? Math.Abs(xpRewardValue - existingXpRewardValue) : -Math.Abs(xpRewardValue - existingXpRewardValue);
            creator.CurrentXp += difference;

            await _uow.CompleteAsync();
            return Unit.Default;
        }
    }
}
