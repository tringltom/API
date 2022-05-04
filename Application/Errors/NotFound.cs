using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Application.Errors
{
    public class NotFound : RestError
    {
        public NotFound(string message) : base(HttpStatusCode.NotFound, new { errors = new { error = message } }) { }
        public override IActionResult Response() => new NotFoundObjectResult(Errors);
    }
}
