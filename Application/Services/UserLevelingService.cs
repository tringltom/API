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
        private readonly IActivityReviewXpRepository _activityReviewXpRepository;
        private readonly IUserRepository _userRepository;

        public UserLevelingService(IActivityReviewXpRepository activityReviewXpRepository, IUserRepository userRepository)
        {
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
                return await _activityReviewXpRepository.GetXpRewardByActivityAndReviewTypeIdsAsync(userReview.Activity.ActivityTypeId, userReview.ReviewTypeId);
            }
            catch (Exception)
            {
                throw new RestException(HttpStatusCode.BadRequest, new { Activity = "Greška pri pronalaženju dobijenih iskustvenih poena." });
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
