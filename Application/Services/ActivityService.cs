using System.Threading.Tasks;
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

            await _activityRepository.CreateActivityAsync(activity);
        }
    }
}
