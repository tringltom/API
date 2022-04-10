using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using Microsoft.AspNetCore.Http;
using static LanguageExt.Prelude;

namespace Application.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IPhotoAccessor _photoAccessor;
        private readonly IUserAccessor _userAccessor;
        private readonly IMapper _mapper;
        private readonly IEmailManager _emailManager;
        private readonly IUnitOfWork _uow;

        public ActivityService(IPhotoAccessor photoAccessor, IUserAccessor userAccessor, IMapper mapper, IEmailManager emailManager, IUnitOfWork uow)
        {
            _photoAccessor = photoAccessor;
            _userAccessor = userAccessor;
            _mapper = mapper;
            _emailManager = emailManager;
            _uow = uow;
        }

        public async Task CreatePendingActivityAsync(ActivityCreate activityCreate)
        {
            var activity = _mapper.Map<PendingActivity>(activityCreate);
            var userId = _userAccessor.GetUserIdFromAccessToken();
            activity.User = await _uow.Users.GetAsync(userId);

            var skill = await _uow.Skills.GetSkill(activity.User.Id, activityCreate.Type);
            var skillActivities = await _uow.SkillActivities.GetAllAsync();
            var maxActivityCounter = skillActivities.FirstOrDefault(sa => sa.Level == (skill?.Level > 3 ? 3 : skill?.Level != null ? skill.Level : 0))?.Counter;

            if (activity.User.ActivityCreationCounters
                .Where(ac => ac.ActivityTypeId == activityCreate.Type && ac.DateCreated.AddDays(7) >= DateTimeOffset.Now)
                .Count() >= maxActivityCounter)
            {
                throw new RestException(HttpStatusCode.BadRequest, new { Greska = "Nemate pravo da kreirate aktivnost" });
            }

            foreach (var image in activityCreate?.Images ?? new IFormFile[0])
            {
                var photoResult = image != null ? await _photoAccessor.AddPhotoAsync(image) : null;
                if (photoResult != null)
                    activity.PendingActivityMedias.Add(new PendingActivityMedia() { PublicId = photoResult.PublicId, Url = photoResult.Url });
            }

            _uow.PendingActivities.Add(activity);

            var activityCreationCounter = new ActivityCreationCounter
            {
                User = activity.User,
                ActivityTypeId = activity.ActivityTypeId,
                DateCreated = DateTimeOffset.Now
            };
            _uow.ActivityCreationCounters.Add(activityCreationCounter);

            if (!await _uow.CompleteAsync())
                throw new RestException(HttpStatusCode.BadRequest, new { Greska = "Greska pri kreiranju aktivnosti" });
        }

        public async Task<PendingActivityEnvelope> GetPendingActivitiesAsync(int? limit, int? offset)
        {
            var pendingActivities = await _uow.PendingActivities.GetLatestPendingActivities(limit, offset);

            return new PendingActivityEnvelope
            {
                Activities = pendingActivities.Select(pa => _mapper.Map<PendingActivityReturn>(pa)).ToList(),
                ActivityCount = await _uow.PendingActivities.CountAsync()
            };
        }

        public async Task<PendingActivityForUserEnvelope> GetPendingActivitiesForLoggedInUserAsync(int? limit, int? offset)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();
            var pendingActivities = await _uow.PendingActivities.GetLatestPendingActivities(userId, limit, offset);

            return new PendingActivityForUserEnvelope
            {
                Activities = pendingActivities.Select(pa => _mapper.Map<PendingActivityForUserReturn>(pa)).ToList(),
                ActivityCount = await _uow.PendingActivities.CountPendingActivities(userId)
            };
        }

        public async Task<bool> ReslovePendingActivityAsync(int pendingActivityId, PendingActivityApproval approval)
        {
            var pendingActivity = await _uow.PendingActivities.GetAsync(pendingActivityId)
                ?? throw new NotFound("Aktivnost nije pronadjena");

            var activity = _mapper.Map<Activity>(pendingActivity);

            if (approval.Approve)
                _uow.Activities.Add(activity);
            else
                activity.ActivityMedias.ToList().ForEach(async m => await _photoAccessor.DeletePhotoAsync(m.PublicId));

            _uow.PendingActivities.Remove(pendingActivity);

            var result = await _uow.CompleteAsync();

            if (result)
            {
                await _emailManager.SendActivityApprovalEmailAsync(pendingActivity.Title, pendingActivity.User.Email, approval.Approve);
                return true;
            }

            return false;
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

        public async Task<Either<RestException, Activity>> ApprovePendingActivity(int id)
        {
            var pendingActivity = await _uow.PendingActivities.GetAsync(id);

            if (pendingActivity == null)
                return new NotFound("Aktivnost nije pronadjena");

            var activity = _mapper.Map<Activity>(pendingActivity);

            _uow.Activities.Add(activity);
            _uow.PendingActivities.Remove(pendingActivity);
            await _uow.CompleteAsync();

            await _emailManager.SendActivityApprovalEmailAsync(pendingActivity.Title, pendingActivity.User.Email, true);
            return activity;
        }

        public async Task<ApprovedActivityReturn> GetActivityAsync(int id)
        {
            var activity = await _uow.Activities.GetAsync(id);
            var activityReturn = _mapper.Map<ApprovedActivityReturn>(activity);
            return activityReturn;
        }
    }
}
