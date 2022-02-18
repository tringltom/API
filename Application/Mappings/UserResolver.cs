using Application.InfrastructureInterfaces.Security;
using Application.Models.Activity;
using AutoMapper;
using Domain;

namespace Application.Mappings
{
    public class UserResolver : IValueResolver<ActivityCreate, PendingActivity, User>
    {
        private readonly IUserAccessor _userAccessor;

        public UserResolver(IUserAccessor userAccessor)
        {
            _userAccessor = userAccessor;
        }

        public User Resolve(ActivityCreate source, PendingActivity destination, User destMember, ResolutionContext context)
        {
            return _userAccessor.FindUserFromAccessToken().Result;
        }
    }
}
