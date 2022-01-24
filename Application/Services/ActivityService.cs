using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Errors;
using Application.Media;
using Application.Repositories;
using Application.ServiceInterfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Models.Activity;

namespace Application.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IPhotoAccessor _photoAccessor;
        private readonly IActivityRepository _activityRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public ActivityService(IPhotoAccessor photoAccessor, IActivityRepository activityRepository, IMapper mapper, IEmailService emailService)
        {
            _photoAccessor = photoAccessor;
            _activityRepository = activityRepository;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task CreatePendingActivityAsync(ActivityCreate activityCreate)
        {
            var activity = _mapper.Map<PendingActivity>(activityCreate);

            if (activity.User.ActivityCreationCounters
                .Where(ac => ac.ActivityTypeId == activityCreate.Type && ac.DateCreated.AddDays(7) >= DateTimeOffset.Now)
                .Count() >= 2)
            {
                throw new RestException(HttpStatusCode.BadRequest, new { Greska = $"Nemate pravo da kreirate aktivnost" });
            }

            foreach (var image in activityCreate?.Images ?? new IFormFile[0])
            {
                var photoResult = image != null ? await _photoAccessor.AddPhotoAsync(image) : null;
                if (photoResult != null)
                    activity.PendingActivityMedias.Add(new PendingActivityMedia() { PublicId = photoResult.PublicId, Url = photoResult.Url });
            }

            await _activityRepository.CreatePendingActivityAsync(activity);

            var activityCreationCounter = new ActivityCreationCounter
            {
                User = activity.User,
                ActivityTypeId = activity.ActivityTypeId,
                DateCreated = DateTimeOffset.Now
            };

            await _activityRepository.CreateActivityCreationCounter(activityCreationCounter);
        }

        public async Task<PendingActivityEnvelope> GetPendingActivitiesAsync(int? limit, int? offset)
        {
            var pendingActivities = await _activityRepository.GetPendingActivitiesAsync(limit, offset);

            return new PendingActivityEnvelope
            {
                Activities = pendingActivities.Select(pa => _mapper.Map<PendingActivityGet>(pa)).ToList(),
                ActivityCount = await _activityRepository.GetPendingActivitiesCountAsync(),
            };
        }

        public async Task<bool> ReslovePendingActivityAsync(int pendingActivityID, PendingActivityApproval approval)
        {
            var pendingActivity = await _activityRepository.GetPendingActivityByIDAsync(pendingActivityID)
                ?? throw new NotFound("Aktivnost nije pronadjena");

            var activity = _mapper.Map<Activity>(pendingActivity);

            if (approval.Approve)
                await _activityRepository.CreatActivityAsync(activity);
            else
                activity.ActivityMedias.ToList().ForEach(async m => await _photoAccessor.DeletePhotoAsync(m.PublicId));

            await _emailService.SendActivityApprovalEmailAsync(pendingActivity, approval.Approve);

            return await _activityRepository.DeletePendingActivity(pendingActivity);
        }
    }
}
