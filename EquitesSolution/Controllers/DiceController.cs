using System.Threading.Tasks;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers
{
    [Route("dice")]
    public class DiceController : BaseController
    {
        private readonly IDiceService _diceService;

        public DiceController(IDiceService diceService)
        {
            _diceService = diceService;
        }

        [HttpGet("rollTheDice")]
        public async Task<ActionResult<DiceResult>> RollTheDice()
        {
            return await _diceService.GetDiceRollResult();
        }
    }
}
