using System.Threading.Tasks;
using Application.Errors;
using Application.Models.User;
using LanguageExt;

namespace Application.ServiceInterfaces
{
    public interface IUserRegistrationService
    {
        Task<Either<RestError, UserBaseResponse>> RegisterAsync(UserRegister user, string origin);
        Task<Either<RestError, Unit>> SendConfirmationEmailAsync(string email, string origin);
        Task<Either<RestError, Unit>> VerifyEmailAsync(UserEmailVerification userEmailVerify);
    }
}
