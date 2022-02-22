﻿using System;
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
using Microsoft.AspNetCore.Http;

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

            if (activity.User.ActivityCreationCounters
                .Where(ac => ac.ActivityTypeId == activityCreate.Type && ac.DateCreated.AddDays(7) >= DateTimeOffset.Now)
                .Count() >= 2)
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
                ActivityCount = await _uow.PendingActivities.CountAsync(pa => pa.User.Id == userId)
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

            await _emailManager.SendActivityApprovalEmailAsync(pendingActivity, approval.Approve);

            _uow.PendingActivities.Remove(pendingActivity);

            return await _uow.CompleteAsync();
        }

        public async Task<ApprovedActivityEnvelope> GetApprovedActivitiesFromOtherUsersAsync(int userId, int? limit, int? offset)
        {
            var approvedActivities = await _uow.Activities.FindAsync(limit, offset, a => a.User.Id != userId, a => a.Id);

            return new ApprovedActivityEnvelope
            {
                Activities = approvedActivities.Select(pa => _mapper.Map<ApprovedActivityReturn>(pa)).ToList(),
                ActivityCount = await _uow.Activities.CountAsync(a => a.User.Id != userId)
            };
        }

    }
}
