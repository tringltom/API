using System.Linq;
using System.Threading.Tasks;
using Application.Errors;
using Application.Media;
using Application.Repositories;
using AutoMapper;
using Domain.Entities;
using Models.Activity;

namespace Application.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IPhotoAccessor _photoAccessor;
        private readonly IActivityRepository _activityRepository;
        private readonly IMapper _mapper;

        public ActivityService(IPhotoAccessor photoAccessor, IActivityRepository activityRepository, IMapper mapper)
        {
            _photoAccessor = photoAccessor;
            _activityRepository = activityRepository;
            _mapper = mapper;
        }

        public async Task CreateActivityAsync(ActivityCreate activityCreate)
        {
            var activity = _mapper.Map<PendingActivity>(activityCreate);

            var photoResult = activityCreate.Image != null ? await _photoAccessor.AddPhotoAsync(activityCreate.Image) : null;

            if (photoResult != null)
                activity.PendingActivityMedias.Add(new PendingActivityMedia() { PublicId = photoResult.PublicId, Url = photoResult.Url });

            await _activityRepository.CreatePendingActivityAsync(activity);
        }

        public async Task<PendingActivityEnvelope> GetPendingActivitiesAsync(int? limit, int? offset)
        {
            var pendingActivities = await _activityRepository.GetPendingActivitiesAsync(limit, offset);

            return new PendingActivityEnvelope
            {
                Activities = pendingActivities,
                ActivityCount = await _activityRepository.GetPendingActivitiesCountAsync(),
            };
        }

        public async Task<bool> ReslovePendingActivityAsync(int pendingActivityID, bool approve)
        {
            var pendingActivity = await _activityRepository.GetPendingActivityByIDAsync(pendingActivityID)
                ?? throw new NotFound("Aktivnost nije pronadjena");

            var activity = _mapper.Map<Activity>(pendingActivity);

            if (approve)
                await _activityRepository.CreatActivityAsync(activity);
            else
                activity.ActivityMedias.ToList().ForEach(async m => await _photoAccessor.DeletePhotoAsync(m.PublicId));

            return await _activityRepository.DeletePendingActivity(pendingActivity);
        }

    }
}
