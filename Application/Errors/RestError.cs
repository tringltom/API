using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Application.Errors
{
    public class RestError
    {
        public RestError(HttpStatusCode code, object errors = null)
        {
            Code = code;
            Errors = errors;
        }

        public HttpStatusCode Code { get; }
        public object Errors { get; }

        public virtual IActionResult Response()
        {
            return null;
        }

    }
}
