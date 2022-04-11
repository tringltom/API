using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Application.Errors
{
    public class RestException : Exception
    {
        public RestException(HttpStatusCode code, object errors = null, Exception exception = null)
        {
            Code = code;
            Errors = errors;
            Exception = exception;
        }

        public HttpStatusCode Code { get; }
        public object Errors { get; }
        public Exception Exception { get; set; }

        public virtual IActionResult Response()
        {
            return new OkObjectResult(Errors);
        }

    }
}
