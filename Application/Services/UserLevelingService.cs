using System;
using System.Net;
using System.Threading.Tasks;
using Application.Errors;
using Application.RepositoryInterfaces;
using Application.ServiceInterfaces;
using Domain.Entities;

namespace Application.Services
{
    public class UserLevelingService : IUserLevelingService
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IActivityReviewXpRepository _activityReviewXpRepository;
        private readonly IUserRepository _userRepository;

        public UserLevelingService(IActivityRepository activityRepository, IActivityReviewXpRepository activityReviewXpRepository, IUserRepository userRepository)
        {
            _activityRepository = activityRepository;
            _activityReviewXpRepository = activityReviewXpRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> ReviewerExistsAsync(int reviewerId)
        {
            return await _userRepository.GetUserByIdAsync(reviewerId) != null;
        }

        public async Task<int> GetXpRewardYieldByReviewAsync(UserReview userReview)
        {
            try
            {
                var activityType = await _activityRepository.GetTypeOfActivityAsync(userReview.ActivityId);

                return await _activityReviewXpRepository.GetXpRewardByActivityAndReviewTypeIdsAsync(activityType, (int)userReview.ReviewTypeId);
            }
            catch (Exception)
            {
                throw new RestException(HttpStatusCode.BadRequest, new { Activity = "Greška pri pronalaženju dobijenih iskustvenih poena za odabranu aktivnost." });
            }
        }

        public async Task UpdateUserXpAsync(int amount, int userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                user.CurrentXp += amount;
                await _userRepository.UpdateUserAsync(user);
            }
            catch (Exception)
            {
                throw new RestException(HttpStatusCode.BadRequest, new { Activity = "Greška pri izmeni dobijenih iskustvenih poena za odabranu aktivnost." });
            }
        }
    }
}
