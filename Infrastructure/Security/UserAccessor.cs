using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Application.InfrastructureInterfaces.Security;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Security
{
    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUsernameFromAccesssToken() => _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        public int GetUserIdFromAccessToken() => Convert.ToInt32(_httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid)?.Value);
    }
}
