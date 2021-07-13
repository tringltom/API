using Application.Models;
using System.Threading.Tasks;

namespace Application.Security
{
    public interface IFacebookAccessor
    {
        Task<FacebookUserInfo> FacebookLogin(string accessToken);
    }
}
