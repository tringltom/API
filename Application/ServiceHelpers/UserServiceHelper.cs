using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace Application.ServiceHelpers;

public class UserServiceHelper : IUserServiceHelper
{
    public string DecodeToken(string token)
    {
        var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
        var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

        return decodedToken;
    }
}

