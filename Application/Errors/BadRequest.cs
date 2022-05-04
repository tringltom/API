using Microsoft.AspNetCore.Mvc;

namespace Application.Errors
{
    public class BadRequest : RestError
    {
        public BadRequest(string message) : base(System.Net.HttpStatusCode.BadRequest, new { errors = new { error = message } }) { }
        public override IActionResult Response() => new BadRequestObjectResult(Errors);
    }
}
