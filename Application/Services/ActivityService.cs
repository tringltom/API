using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces;
using Application.InfrastructureInterfaces.Security;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using AutoMapper;
using DAL;
using Domain;
using LanguageExt;

namespace Application.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IUserAccessor _userAccessor;
        private readonly IMapper _mapper;
        private readonly IEmailManager _emailManager;
        private readonly IUnitOfWork _uow;

        public ActivityService(IUserAccessor userAccessor, IMapper mapper, IEmailManager emailManager, IUnitOfWork uow)
        {
            _userAccessor = userAccessor;
            _mapper = mapper;
            _emailManager = emailManager;
            _uow = uow;
        }

        public async Task<ApprovedActivityReturn> GetActivityAsync(int id)
        {
            var activity = await _uow.Activities.GetAsync(id);
            return _mapper.Map<ApprovedActivityReturn>(activity);
        }

        public async Task<ApprovedActivityEnvelope> GetActivitiesFromOtherUsersAsync(int? limit, int? offset)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();

            var activities = await _uow.Activities.GetOrderedActivitiesFromOtherUsersAsync(limit, offset, userId);

            return new ApprovedActivityEnvelope
            {
                Activities = _mapper.Map<IEnumerable<Activity>, IEnumerable<ApprovedActivityReturn>>(activities).ToList(),
                ActivityCount = await _uow.Activities.CountOtherUsersActivitiesAsync(userId)
            };
        }

        public async Task<Either<RestError, ApprovedActivityReturn>> ApprovePendingActivity(int id)
        {
            var pendingActivity = await _uow.PendingActivities.GetAsync(id);

            if (pendingActivity == null)
                return new NotFound("Aktivnost nije pronadjena");

            var activity = _mapper.Map<Activity>(pendingActivity);

            _uow.Activities.Add(activity);
            _uow.PendingActivities.Remove(pendingActivity);
            await _uow.CompleteAsync();

            await _emailManager.SendActivityApprovalEmailAsync(pendingActivity.Title, pendingActivity.User.Email, true);

            return _mapper.Map<ApprovedActivityReturn>(activity);
        }
    }
}
