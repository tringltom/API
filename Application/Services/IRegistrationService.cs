using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IRegistrationService
    {
        Task Register(User user, string password, string origin);
        Task ResendConfirmationEmail(string email, string origin);
    }
}
