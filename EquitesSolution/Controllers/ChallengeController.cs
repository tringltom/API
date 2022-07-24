using System.Threading.Tasks;
using API.Validations;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using DAL.Query;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("challenges")]
    public class ChallengeController : BaseController
    {
        private readonly IChallengeService _challengeService;

        public ChallengeController(IChallengeService challengeService)
        {
            _challengeService = challengeService;
        }

        [HttpGet("me/challenge-answers/activity/{id}")]
        [IdValidation]
        public async Task<IActionResult> GetOwnerChallengeAnswers(int id, [FromQuery] QueryObject queryObject)
        {
            var result = await _challengeService.GetOwnerChallengeAnswersAsync(id, queryObject);

            return result.Match(
                challengeAnswers => Ok(challengeAnswers),
                err => err.Response()
                );
        }

        // TODO - Add checking if user is Admin
        [HttpGet("pending-challenges")]
        public async Task<IActionResult> GetChallengesForApproval([FromQuery] QueryObject queryObject)
        {
            return Ok(await _challengeService.GetChallengesForApprovalAsync(queryObject));
        }

        [HttpPatch("challenge-confirmation/{id}")]
        [IdValidation]
        public async Task<IActionResult> ConfirmChallengeAnswer(int id)
        {
            var result = await _challengeService.ConfirmChallengeAnswerAsync(id);

            return result.Match(
               u => Ok(),
               err => err.Response()
               );
        }

        // TODO - Add checking if user is Admin
        [HttpPatch("challenge-answer-disapproval/{id}")]
        [IdValidation]
        public async Task<IActionResult> DisapproveChallengeAnswer(int id)
        {
            var result = await _challengeService.DisapproveChallengeAnswerAsync(id);

            return result.Match(
               u => Ok(),
               err => err.Response()
               );
        }

        [HttpPost("{id}/challenge-answer")]
        [IdValidation]
        public async Task<IActionResult> AnswerToChallenge(int id, [FromForm] ChallengeAnswer challengeAnswer)
        {
            var result = await _challengeService.AnswerToChallengeAsync(id, challengeAnswer);

            return result.Match(
               u => Ok(),
               err => err.Response()
               );
        }

        // TODO - Add checking if user is Admin
        [HttpPost("challenge-answer-approval/{id}")]
        [IdValidation]
        public async Task<IActionResult> ApproveChallengeAnswer(int id)
        {
            var result = await _challengeService.ApproveChallengeAnswerAsync(id);

            return result.Match(
               u => Ok(),
               err => err.Response()
               );
        }
    }
}
