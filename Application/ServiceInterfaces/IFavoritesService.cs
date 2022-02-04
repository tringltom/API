using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Activity;

namespace Application.ServiceInterfaces
{
    public interface IFavoritesService
    {
        Task CreateFavoriteAsync(FavoriteActivityBase activity);
        Task<IList<FavoriteActivityReturn>> GetAllFavoritesForUserAsync(int userId);
        Task RemoveFavoriteAsync(FavoriteActivityBase favoriteActivity);
    }
}
