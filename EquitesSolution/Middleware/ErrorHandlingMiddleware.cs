using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace API.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task HandleExceptionAsync(HttpContext context, Exception ex, ILogger<ErrorHandlingMiddleware> logger)
    {
        object errors = null;

        switch (ex)
        {
            case RestException re:
                if (re.Exception != null)
                    logger.LogCritical(re, "REST ERROR - {error}, INNER ERROR - {@innerError}", re.Errors, re.Exception);
                else
                    logger.LogError(re, "REST ERROR - {error}", re.Errors);

                errors = re.Errors;
                context.Response.StatusCode = (int)re.Code;
                break;

            case Exception e:
                logger.LogCritical(e, "SERVER ERROR");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        context.Response.ContentType = "application/json";
        if (errors != null)
        {
            var result = JsonSerializer.Serialize(new { errors });

            await context.Response.WriteAsync(result);
        }
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, _logger);
        }
    }
}
