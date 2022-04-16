using System.Threading.Tasks;
using Application.Errors;
using Application.Models.User;
using LanguageExt;

namespace Application.ServiceInterfaces
{
    public interface IUserSessionService
    {
        Task<Either<RestError, UserBaseResponse>> LoginAsync(UserLogin userLogin);
        Task<Either<RestError, UserRefreshResponse>> RefreshTokenAsync(string refreshToken);
        //Task<UserBaseResponse> FacebookLoginAsync(string accessToken, CancellationToken cancellationToken);
        Task<Either<RestError, UserBaseResponse>> GetCurrentlyLoggedInUserAsync(string stayLoggedIn, string refreshToken);
        Task<Either<RestError, Unit>> LogoutUserAsync(string refreshToken);
    }
}
