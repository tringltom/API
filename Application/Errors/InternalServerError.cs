using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Errors
{
    public class InternalServerError : RestError
    {
        public InternalServerError(string message) : base(HttpStatusCode.InternalServerError, new { error = message }) { }
        public override IActionResult Response() => new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }
}
