using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.InfrastructureInterfaces.Security;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Security
{
    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userIdentityManager;

        public UserAccessor(IHttpContextAccessor httpContextAccessor, UserManager<User> userIdentityManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userIdentityManager = userIdentityManager;
        }

        public async Task<User> FindUserFromAccessToken() => await _userIdentityManager.FindByIdAsync(GetUserIdFromAccessToken().ToString());

        public string GetUsernameFromAccesssToken() => _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        public int GetUserIdFromAccessToken() => Convert.ToInt32(_httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid)?.Value);

    }
}
