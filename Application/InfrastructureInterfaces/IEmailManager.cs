using System.Threading.Tasks;
using Domain;

namespace Application.InfrastructureInterfaces
{
    public interface IEmailManager
    {
        Task SendConfirmationEmailAsync(string verifyUrl, string email);
        Task SendPasswordRecoveryEmailAsync(string verifyUrl, string email);
        Task SendActivityApprovalEmailAsync(PendingActivity activity, bool approved);
        string DecodeVerificationToken(string token);
    }
}
