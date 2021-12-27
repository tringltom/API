using Application.RepositoryInterfaces;

namespace Application.Managers
{
    public class UserManager : IUserManager
    {

        private readonly IUserRepository _userRepository;

        public UserManager(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
    }
}
