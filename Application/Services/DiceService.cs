using System;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces.Security;
using Application.Models;
using Application.ServiceInterfaces;
using DAL;
using LanguageExt;

namespace Application.Services
{
    public class DiceService : IDiceService
    {

        private readonly IUserAccessor _userAccessor;
        private readonly IUnitOfWork _uow;

        public DiceService(IUserAccessor userAccessor, IUnitOfWork uow)
        {
            _userAccessor = userAccessor;
            _uow = uow;
        }

        public async Task<Either<RestError, DiceResult>> GetDiceRollResult()
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();
            var user = await _uow.Users.GetAsync(userId);

            if (user.LastRollDate != null && (DateTimeOffset.Now - user.LastRollDate) < TimeSpan.FromDays(1))
                return new NotFound("Bacanje kockice je moguće jednom dnevno");

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

            await _uow.CompleteAsync();

            return new DiceResult { Result = diceResult, Message = message };
        }
    }
}
