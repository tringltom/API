using System.Threading;
using System.Threading.Tasks;
using Models.User;

namespace Application.Services
{
    public interface IUserService
    {
        Task RegisterAsync(UserRegister user, string origin);
        Task ResendConfirmationEmailAsync(string email, string origin);
        Task ConfirmEmailAsync(UserEmailVerification userEmailVerify);
        Task RecoverUserPasswordViaEmailAsync(string email, string origin);
        Task ConfirmUserPasswordRecoveryAsync(UserPasswordRecoveryVerification userPasswordRecovery);
        Task ChangeUserPasswordAsync(UserPasswordChange userPassChange);
        Task<UserBaseResponse> LoginAsync(UserLogin userLogin);
        Task<UserBaseResponse> RefreshTokenAsync(string refreshToken);
        Task<UserBaseResponse> FacebookLoginAsync(string accessToken, CancellationToken cancellationToken);
        Task<UserCurrentlyLoggedIn> GetCurrentlyLoggedInUserAsync();
        Task LogoutUserAsync(string refreshToken);
    }
}
