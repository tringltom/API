using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Application.Errors
{
    public class NotAuthorized : RestError
    {
        public NotAuthorized(string message) : base(HttpStatusCode.Unauthorized, new { errors = new { error = message } }) { }
        public override IActionResult Response() => new UnauthorizedObjectResult(Errors);
    }
}
