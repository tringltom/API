using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces.Security;
using Application.Models.User;
using Application.ServiceInterfaces;
using AutoMapper;
using DAL;
using Domain;

namespace Application.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IUserAccessor _userAccessor;

        public UsersService(IUnitOfWork uow, IMapper mapper, IUserAccessor userAccessor)
        {
            _uow = uow;
            _mapper = mapper;
            _userAccessor = userAccessor;
        }

        public async Task<UserRangingEnvelope> GetRangingUsers(int? limit, int? offset)
        {
            var topXpUsers = await _uow.Users.GetRangingUsers(limit, offset);

            var userRangingEnvelope = new UserRangingEnvelope
            {
                Users = _mapper.Map<IEnumerable<User>, IEnumerable<UserRangingGet>>(topXpUsers).ToList(),
                UserCount = await _uow.Users.CountAsync(),
            };

            return userRangingEnvelope;
        }

        public async Task UpdateLoggedUserAbout(UserAbout userAbout)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();
            var user = await _uow.Users.GetAsync(userId);

            if (user == null)
                throw new NotFound("Nepostojeci korisnik");

            user.About = userAbout.About;

            await _uow.CompleteAsync();
        }
    }
}
