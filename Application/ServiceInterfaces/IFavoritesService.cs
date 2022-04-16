using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Errors;
using Application.Models.Activity;
using LanguageExt;

namespace Application.ServiceInterfaces
{
    public interface IFavoritesService
    {
        Task<UserFavoriteActivityReturn> GetFavoriteActivityAsync(int id);
        Task<IList<FavoriteActivityIdReturn>> GetAllOwnerFavoriteIds();
        Task<Either<RestError, Unit>> RemoveFavoriteActivityAsync(int activityId);
        Task<Either<RestError, UserFavoriteActivityReturn>> AddFavoriteActivityAsync(int activityId);
    }
}
