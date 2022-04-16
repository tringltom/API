using System.Threading.Tasks;
using Application.Errors;
using Application.Models.User;
using LanguageExt;

namespace Application.ServiceInterfaces
{
    public interface IUserRecoveryService
    {
        Task<Either<RestError, Unit>> RecoverUserPasswordViaEmailAsync(string email, string origin);
        Task<Either<RestError, Unit>> ConfirmUserPasswordRecoveryAsync(UserPasswordRecoveryVerification userPasswordRecovery);
    }
}
