using System.Threading.Tasks;
using Models;

namespace Application.ServiceInterfaces
{
    public interface IDiceService
    {
        Task<DiceResult> GetDiceRollResult();
    }
}
