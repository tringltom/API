using System.Linq;
using Application.InfrastructureInterfaces.Security;
using Application.Models.Activity;
using AutoMapper;
using Domain;

namespace Application.Mappings
{
    public class AtendeeResolver : IValueResolver<Activity, OtherUserActivityReturn, bool>
    {
        private readonly IUserAccessor _userAccessor;

        public AtendeeResolver(IUserAccessor userAccessor)
        {
            _userAccessor = userAccessor;
        }

        public bool Resolve(Activity source, OtherUserActivityReturn destination, bool destMember, ResolutionContext context)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();
            return source.UserAttendances != null && source.UserAttendances.Any(ua => ua.UserId == userId);
        }
    }
}
