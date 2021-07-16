using System.Threading.Tasks;

namespace Application.Services
{
    public interface IEmailService
    {
        Task SendConfirmationEmailAsync(string verifyUrl, string email);
        Task SendPasswordRecoveryEmailAsync(string verifyUrl, string email);
    }
}
