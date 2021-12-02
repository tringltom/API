using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Activity;

namespace API.Controllers;

[Route("activities")]
public class ActivityController : BaseController
{
    private readonly IActivityService _activityService;

    public ActivityController(IActivityService activityService)
    {
        _activityService = activityService;
    }

    [HttpPost("create")]
    public async Task<ActionResult> CreateActivity([FromForm] ActivityCreate activityCreate)
    {
        await _activityService.CreateActivityAsync(activityCreate);

        return Ok("Uspešno kreiranje, molimo Vas da sačekate odobrenje");
    }
}

