using Application.RepositoryInterfaces;
using AutoMapper;
using Domain.Entities;
using Models.Activity;

namespace Application.Mappings
{
    public class UserResolver : IValueResolver<ActivityCreate, PendingActivity, User>, IValueResolver<FavoriteActivityCreate, UserFavoriteActivity, User>
    {
        private readonly IUserRepository _userRepository;

        public UserResolver(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User Resolve(ActivityCreate source, PendingActivity destination, User destMember, ResolutionContext context)
        {
            return _userRepository.GetUserUsingTokenAsync().Result;
        }

        public User Resolve(FavoriteActivityCreate source, UserFavoriteActivity destination, User destMember, ResolutionContext context)
        {
            return _userRepository.GetUserByIdAsync(source.UserId).Result;
        }
    }
}
