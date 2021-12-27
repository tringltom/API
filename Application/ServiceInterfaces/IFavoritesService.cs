using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Models.Activity;

namespace Application.ServiceInterfaces
{
    public interface IFavoritesService
    {
        Task CreateFavoriteAsync(FavoriteActivityCreate activity);
        Task<IList<UserFavoriteActivity>> GetAllFavoritesForUserAsync(int userId);
        Task RemoveFavoriteAsync(FavoriteActivityRemove favoriteActivity);
    }
}
