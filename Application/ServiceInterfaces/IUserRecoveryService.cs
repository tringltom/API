using System.Threading.Tasks;
using Application.Models.User;

namespace Application.ServiceInterfaces
{
    public interface IUserRecoveryService
    {
        Task RecoverUserPasswordViaEmailAsync(string email, string origin);
        Task ConfirmUserPasswordRecoveryAsync(UserPasswordRecoveryVerification userPasswordRecovery);
        Task ChangeUserPasswordAsync(UserPasswordChange userPassChange);
    }
}
