using System.Threading.Tasks;
using Application.Models;

namespace Application.ServiceInterfaces
{
    public interface IDiceService
    {
        Task<DiceResult> GetDiceRollResult();
    }
}
