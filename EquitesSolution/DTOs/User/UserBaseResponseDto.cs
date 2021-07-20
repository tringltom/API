using Application.Security;
using System.Text.Json.Serialization;

namespace API.DTOs.User
{
    public class UserBaseResponseDto
    {
            public string Token { get; set; }
            public string Username { get; set; }

            [JsonIgnore]
            public string RefreshToken { get; set; }

    }
}
