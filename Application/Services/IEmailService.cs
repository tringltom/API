using System.Threading.Tasks;

namespace Application.Services
{
    public interface IEmailService
    {
        Task<bool> SendConfirmationEmailAsync(string verifyUrl, string email);
    }
}
