﻿using System.Threading.Tasks;
using API.Validations;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using DAL.Query;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("activities")]
    public class ActivityController : BaseController
    {
        private readonly IActivityService _activityService;

        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpGet("{id}", Name = nameof(GetActivity))]
        [IdValidation]
        public async Task<IActionResult> GetActivity(int id)
        {
            return Ok(await _activityService.GetActivityAsync(id));
        }

        [HttpGet("others")]
        public async Task<IActionResult> GetActivitiesFromOtherUsers([FromQuery] ActivityQuery activityQuery)
        {
            return Ok(await _activityService.GetActivitiesFromOtherUsersAsync(activityQuery));
        }

        [HttpPatch("{id}/answer")]
        public async Task<IActionResult> AnswerToPuzzle(int id, PuzzleAnswer puzzleAnswer)
        {
            var result = await _activityService.AnswerToPuzzleAsync(id, puzzleAnswer);

            return result.Match(
               xpReward => Ok(xpReward),
               err => err.Response()
               );
        }

        // TODO - Add checking if user is Admin/Approver
        [HttpPost("pending-activity/{id}")]
        [IdValidation]
        public async Task<IActionResult> ApprovePendingActivity(int id)
        {
            var result = await _activityService.ApprovePendingActivity(id);

            return result.Match(
                activity => CreatedAtRoute(nameof(GetActivity), new { id = activity.Id }, activity),
                err => err.Response()
                );
        }

        [HttpGet("approved-activities", Name = nameof(GetApprovedActivitiesForUser))]
        public async Task<IActionResult> GetApprovedActivitiesForUser([FromQuery] UserQuery userQuery)
        {
            var result = await _activityService.GetApprovedActivitiesForUserAsync(userQuery);

            return result.Match(
                approvedActivitiesEnvelope => Ok(approvedActivitiesEnvelope),
                err => err.Response()
                );
        }
    }
}
