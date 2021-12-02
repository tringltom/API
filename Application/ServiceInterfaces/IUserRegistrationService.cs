using System.Threading.Tasks;
using Models.User;

namespace Application.ServiceInterfaces;

public interface IUserRegistrationService
{
    Task RegisterAsync(UserRegister user, string origin);
    Task ResendConfirmationEmailAsync(string email, string origin);
    Task ConfirmEmailAsync(UserEmailVerification userEmailVerify);
}

