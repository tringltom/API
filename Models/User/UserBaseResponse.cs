
using System.Text.Json.Serialization;

namespace Models.User;

public class UserBaseResponse
{
    public UserBaseResponse(string token, string userName, string refreshToken)
    {
        Token = token;
        Username = userName;
        RefreshToken = refreshToken;
    }

    public string Token { get; set; }
    public string Username { get; set; }

    [JsonIgnore]
    public string RefreshToken { get; set; }
}

