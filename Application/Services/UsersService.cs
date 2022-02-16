using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.ServiceInterfaces;
using AutoMapper;
using DAL;
using Domain;
using Models.User;

namespace Application.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public UsersService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<UserRangingEnvelope> GetTopXpUsers(int? limit, int? offset)
        {
            var topXpUsers = await _uow.Users.GetTopXpUsersAsync(limit, offset);

            var userArenaEnvelope = new UserRangingEnvelope
            {
                Users = _mapper.Map<IEnumerable<User>, IEnumerable<UserRangingGet>>(topXpUsers).ToList(),
                UserCount = await _uow.Users.CountAsync(),
            };

            return userArenaEnvelope;
        }

    }
}
