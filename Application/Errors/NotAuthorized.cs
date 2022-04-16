using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Errors
{
    public class NotAuthorized : RestError
    {
        public NotAuthorized(string message) : base(HttpStatusCode.Unauthorized, new { error = message }) { }
        public override IActionResult Response() => new StatusCodeResult(StatusCodes.Status401Unauthorized);
    }
}
