using System;
using System.Net;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces;
using Application.ServiceInterfaces;
using DAL.RepositoryInterfaces;
using Domain;

namespace Application.Services
{
    public class UserLevelingService : IUserLevelingService
    {
        private readonly IActivityReviewXpRepository _activityReviewXpRepository;
        private readonly IUserManager _userManager;

        public UserLevelingService(IActivityReviewXpRepository activityReviewXpRepository, IUserManager userManager)
        {
            _activityReviewXpRepository = activityReviewXpRepository;
            _userManager = userManager;
        }

        public async Task<bool> ReviewerExistsAsync(int reviewerId)
        {
            return await _userManager.FindUserByIdAsync(reviewerId) != null;
        }

        public async Task<int> GetXpRewardYieldByReviewAsync(ActivityTypeId activityTypeId, ReviewTypeId reviewTypeId)
        {
            try
            {
                return await _activityReviewXpRepository.GetXpRewardByActivityAndReviewTypeIdsAsync(activityTypeId, reviewTypeId);
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
                var user = await _userManager.FindUserByIdAsync(userId);
                user.CurrentXp += amount;
                await _userManager.UpdateUserAsync(user);
            }
            catch (Exception)
            {
                throw new RestException(HttpStatusCode.BadRequest, new { Activity = "Greška pri izmeni dobijenih iskustvenih poena za odabranu aktivnost." });
            }
        }
    }
}
