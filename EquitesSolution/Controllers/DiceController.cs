﻿using System.Threading.Tasks;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("roll")]
        public async Task<IActionResult> RollTheDice()
        {
            var result = await _diceService.GetDiceRollResult();

            return result.Match(
               diceResult => Ok(diceResult),
               err => err.Response()
               );
        }
    }
}
