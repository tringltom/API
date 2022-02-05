using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Repositories;
using Application.RepositoryInterfaces;
using Application.ServiceInterfaces;
using AutoMapper;
using Domain.Entities;
using Models.User;

namespace Application.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserArenaEnvelope> GetTopXpUsers(int? limit, int? offset)
        {
            var topXpUsers = await _userRepository.GetTopXpUsersAsync(limit, offset);

            var userArenaEnvelope = new UserArenaEnvelope
            {
                Users = (List<UserArenaGet>)_mapper.Map<IEnumerable<User>, IEnumerable<UserArenaGet>>(topXpUsers),
                UserCount = await _userRepository.GetUserCountAsync(),
            };

            return userArenaEnvelope;
        }

    }
}
