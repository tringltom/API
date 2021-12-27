using Application.RepositoryInterfaces;
using AutoMapper;
using Domain.Entities;
using Models.Activity;

namespace Application.Mappings
{
    public class ActivityResolver : IValueResolver<FavoriteActivityCreate, UserFavoriteActivity, Activity>
    {
        private readonly IActivityRepository _activityRepository;

        public ActivityResolver(IActivityRepository activityRepository)
        {
            _activityRepository = activityRepository;
        }

        public Activity Resolve(FavoriteActivityCreate source, UserFavoriteActivity destination, Activity destMember, ResolutionContext context)
        {
            return _activityRepository.GetActivityByIdAsync(source.ActivityId).Result;
        }
    }
}
