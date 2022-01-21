using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Application.Errors;
using Application.RepositoryInterfaces;
using AutoMapper;
using Domain.Entities;
using Models.Activity;

namespace Application.ServiceInterfaces
{
    public class FavoritesService : IFavoritesService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFavoritesRepository _favoritesRepository;
        private readonly IMapper _mapper;

        public FavoritesService(IFavoritesRepository favoritesRepository, IMapper mapper, IUserRepository userRepository)
        {
            _favoritesRepository = favoritesRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task CreateFavoriteAsync(FavoriteActivityCreate activity)
        {

            var currentUserName = _userRepository.GetCurrentUsername();

            var currentUser = await _userRepository.GetUserByUserNameAsync(currentUserName);

            if (currentUser.Id != activity.UserId)
            {
                throw new RestException(HttpStatusCode.BadRequest, new { Activity = "Greška, ne možete napraviti omiljenu aktivnost za drugog korisnika." });
            }

            var userFavoriteActivity = _mapper.Map<UserFavoriteActivity>(activity);

            var existingActivity = await _favoritesRepository.GetFavoriteActivityAsync(userFavoriteActivity);

            if (existingActivity != null)
            {
                throw new RestException(HttpStatusCode.BadRequest, new { Activity = "Greška, aktivnost je vec medju omiljenima." });
            }

            try
            {
                await _favoritesRepository.AddFavoriteActivityAsync(userFavoriteActivity);
            }
            catch (Exception)
            {
                throw new RestException(HttpStatusCode.BadRequest, new { FavoriteActivity = "Greška, korisnik i/ili aktivnost su nepostojeći." });
            }
        }

        public async Task<IList<FavoriteActivityReturn>> GetAllFavoritesForUserAsync(int userId)
        {
            var favoriteActivities = await _favoritesRepository.GetFavoriteActivitiesByUserIdAsync(userId);

            return _mapper.Map<List<FavoriteActivityReturn>>(favoriteActivities);
        }

        public async Task RemoveFavoriteAsync(FavoriteActivityRemove favoriteActivity)
        {
            var currentUser = await _userRepository.GetUserByUserNameAsync(_userRepository.GetCurrentUsername());

            if (currentUser.Id != favoriteActivity.UserId)
            {
                throw new RestException(HttpStatusCode.BadRequest, new { Activity = "Greška, ne možete ukloniti omiljenu aktivnost od drugog korisnika." });
            }


            if (!await _favoritesRepository.RemoveFavoriteActivityByActivityAndUserIdAsync(favoriteActivity.UserId, favoriteActivity.ActivityId))
            {
                throw new RestException(HttpStatusCode.BadRequest, new { Activity = "Greška, aktivnost nije medju omiljenima." });
            }
        }
    }
}
