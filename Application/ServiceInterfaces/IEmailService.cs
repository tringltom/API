using System.Threading.Tasks;
using Domain.Entities;

namespace Application.ServiceInterfaces
{
    public interface IEmailService
    {
        Task SendConfirmationEmailAsync(string verifyUrl, string email);
        Task SendPasswordRecoveryEmailAsync(string verifyUrl, string email);
        Task SendActivityApprovalEmailAsync(PendingActivity activity, bool approved);
    }
}
