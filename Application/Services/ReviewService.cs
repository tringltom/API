using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces.Security;
using Application.Models.Activity;
using AutoMapper;
using DAL;
using Domain;

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

        public async Task<IList<UserReviewedActivity>> GetAllReviews(int userId)
        {
            var userReviews = await _uow.UserReviews.GetUserReviews(userId);

            return _mapper.Map<List<UserReviewedActivity>>(userReviews);
        }

        public async Task ReviewActivityAsync(ActivityReview activityReview)
        {
            var reviewerId = _userAccessor.GetUserIdFromAccessToken();

            var activityCreatorId = (await _uow.Activities.GetAsync(activityReview.ActivityId)).User.Id;

            if (reviewerId == activityCreatorId)
                throw new RestException(HttpStatusCode.BadRequest, new { ActivityReview = "Greška, ne možete oceniti svoju aktivnost." });

            var xpRewardToYield = await _uow.ActivityReviewXps.GetXpRewardAsync(activityReview.ActivityTypeId, activityReview.ReviewTypeId);

            var existingReview = await _uow.UserReviews.GetUserReview(activityReview.ActivityId, reviewerId);

            if (existingReview == null)
            {
                var review = _mapper.Map<UserReview>(activityReview);
                review.UserId = reviewerId;
                _uow.UserReviews.Add(review);

                var user = await _uow.Users.GetAsync(activityCreatorId);
                user.CurrentXp += xpRewardToYield.Xp;

                await _uow.CompleteAsync();

                return;
            }

            var existingXpReward = await _uow.ActivityReviewXps.GetXpRewardAsync(existingReview.Activity.ActivityTypeId, existingReview.ReviewTypeId);

            if (existingXpReward.Xp == xpRewardToYield.Xp)
            {
                return;
            }

            existingReview.ReviewTypeId = activityReview.ReviewTypeId;

            var difference = CalculateAmountToChange(xpRewardToYield.Xp, existingXpReward.Xp);

            var userToUpdate = await _uow.Users.GetAsync(activityCreatorId);
            userToUpdate.CurrentXp += difference;

            await _uow.CompleteAsync();
        }

        private int CalculateAmountToChange(int newValue, int oldValue)
        {
            return newValue > oldValue ? Math.Abs(newValue - oldValue) : -Math.Abs(newValue - oldValue);
        }
    }
}
