using Application.InfrastructureInterfaces.Security;
using Application.Models.Activity;
using AutoMapper;
using Domain;

namespace Application.Mappings
{
    public class HostResolver : IValueResolver<Activity, OtherUserActivityReturn, bool>
    {
        private readonly IUserAccessor _userAccessor;

        public HostResolver(IUserAccessor userAccessor)
        {
            _userAccessor = userAccessor;
        }

        public bool Resolve(Activity source, OtherUserActivityReturn destination, bool destMember, ResolutionContext context)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();
            return (source.ActivityTypeId == ActivityTypeId.Happening || source.ActivityTypeId == ActivityTypeId.Challenge)
                && source.User.Id == userId;
        }
    }
}
