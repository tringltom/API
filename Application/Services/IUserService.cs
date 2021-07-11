using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IUserService
    {
        Task RegisterAsync(User user, string password, string origin);
        Task ResendConfirmationEmailAsync(string email, string origin);
        Task ConfirmEmailAsync(string email, string token);
        Task LoginAsync(string email, string password);
    }
}
