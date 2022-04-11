using Microsoft.AspNetCore.Mvc;

namespace Application.Errors
{
    public class BadRequest : RestException
    {
        public BadRequest(string message) : base(System.Net.HttpStatusCode.BadRequest, new { error = message })
        {

        }
        public override IActionResult Response()
        {
            return new BadRequestObjectResult(Errors);
        }
    }
}
