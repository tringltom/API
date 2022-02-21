using System.Threading.Tasks;
using Application.InfrastructureModels;

namespace Application.InfrastructureInterfaces.Security
{
    public interface IFacebookAccessor
    {
        Task<FacebookUserInfo> FacebookLogin(string accessToken);
    }
}
