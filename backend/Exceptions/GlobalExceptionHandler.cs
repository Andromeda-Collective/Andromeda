using Microsoft.AspNetCore.Diagnostics;
using Andromeda.Common.Factories;
using Andromeda.Common;

namespace Andromeda.Exceptions;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred");

        if (httpContext.Response.HasStarted)
        {
            logger.LogWarning("Response already started");
            return false;
        }

        var (type, statusCode, message) = MapException(exception);


        var problemDetails = ProblemDetailsFactory.CreateProblemDetails(type, statusCode, Error.Failure("GlobalException", message));
        

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static (string type, int statusCode, string message) MapException(Exception ex)
    {
        return ex switch
        {
            BadHttpRequestException =>
                ("Bad Request", StatusCodes.Status400BadRequest, "Invalid request body"),

            System.Text.Json.JsonException =>
                ("Bad Request", StatusCodes.Status400BadRequest, "Malformed JSON"),

            InvalidOperationException =>
                ("Bad Request", StatusCodes.Status400BadRequest, "Invalid request"),

            _ =>
                ("Internal Server Error", StatusCodes.Status500InternalServerError, "Server failure")
        };
    }
}
