using System.Threading.Tasks;
using Application.Models;

namespace Application.Security
{
    public interface IFacebookAccessor
    {
        Task<FacebookUserInfo> FacebookLogin(string accessToken);
    }
}
