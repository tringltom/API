using System.Threading.Tasks;
using Application.SecurityModels;

namespace Application.Security;

public interface IFacebookAccessor
{
    Task<FacebookUserInfo> FacebookLogin(string accessToken);
}
