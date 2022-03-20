using System;
using System.Net;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces.Security;
using Application.ManagerInterfaces;
using Application.Models;
using Application.ServiceInterfaces;

namespace Application.Services
{
    public class DiceService : IDiceService
    {
        private readonly IUserManager _userManagerRepository;
        private readonly IUserAccessor _userAccessor;

        public DiceService(IUserManager userManagerRepostiory, IUserAccessor userAccessor)
        {
            _userManagerRepository = userManagerRepostiory;
            _userAccessor = userAccessor;
        }

        public async Task<DiceResult> GetDiceRollResult()
        {
            var user = await _userAccessor.FindUserFromAccessToken();

            if (user.LastRollDate != null && (DateTimeOffset.Now - user.LastRollDate) < TimeSpan.FromDays(1))
                return new DiceResult { Result = 0, Message = "Bacanje kockice je moguće jednom dnevno" };

            user.LastRollDate = DateTimeOffset.Now;

            var rnd = new Random();
            var diceResult = rnd.Next(1, 21);
            var message = "";

            //TO DO: add additional effects
            switch (diceResult)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    user.CurrentXp += 50;
                    message = "Dobili ste 50 iskustvenih poena!!";
                    break;
                case 11:
                    break;
                case 12:
                    break;
                case 13:
                    break;
                case 14:
                    break;
                case 15:
                    break;
                case 16:
                    break;
                case 17:
                    break;
                case 18:
                    break;
                case 19:
                    break;
                case 20:
                    break;
                default:
                    break;
            }

            if (!await _userManagerRepository.UpdateUserAsync(user))
                throw new RestException(HttpStatusCode.BadRequest, new { Error = "Neuspešno ažuriranje korisnika" });

            return new DiceResult { Result = diceResult, Message = message };
        }
    }
}
