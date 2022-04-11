using System.Threading.Tasks;
using Application.Errors;
using Application.Models;
using LanguageExt;

namespace Application.ServiceInterfaces
{
    public interface IDiceService
    {
        Task<Either<RestException, DiceResult>> GetDiceRollResult();
    }
}
