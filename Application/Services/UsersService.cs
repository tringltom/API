using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Repositories;
using Application.ServiceInterfaces;
using AutoMapper;
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
                Users = topXpUsers.Select(xpu => _mapper.Map<UserArenaGet>(xpu)).ToList(),
                UserCount = await _userRepository.GetUserCountAsync(),
            };

            return userArenaEnvelope;
        }

    }
}
