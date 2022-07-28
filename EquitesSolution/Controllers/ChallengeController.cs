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

        [HttpGet("me/answers/activity/{id}")]
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
        [HttpGet("pending")]
        public async Task<IActionResult> GetChallengesForApproval([FromQuery] QueryObject queryObject)
        {
            return Ok(await _challengeService.GetChallengesForApprovalAsync(queryObject));
        }

        [HttpPatch("confirmation/{id}")]
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
        [HttpPatch("answer-disapproval/{id}")]
        [IdValidation]
        public async Task<IActionResult> DisapproveChallengeAnswer(int id)
        {
            var result = await _challengeService.DisapproveChallengeAnswerAsync(id);

            return result.Match(
               u => Ok(),
               err => err.Response()
               );
        }

        [HttpPost("{id}/answer")]
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
        [HttpPost("answer-approval/{id}")]
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
