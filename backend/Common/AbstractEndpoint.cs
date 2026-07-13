using Andromeda.Common.Factories;
using Andromeda.Enums;

namespace Andromeda.Common;

public abstract class AbstractEndpoint
{
    public abstract void MapEndpoint(IEndpointRouteBuilder app);


    public IResult HandleFailure(Result result) =>
    result switch
    {
        { IsSuccess: true } => throw new InvalidOperationException(),
        IValidationResult validationResult => Results.BadRequest(ProblemDetailsFactory.CreateProblemDetails(
            "Validation Error",
            StatusCodes.Status400BadRequest,
            result.Error,
            validationResult.Errors
        )),
        { Error.Type: ErrorType.NotFound } => Results.NotFound(ProblemDetailsFactory.CreateProblemDetails(
            "Not Found",
            StatusCodes.Status404NotFound,
            result.Error
        )),
        { Error.Type: ErrorType.Conflict } => Results.Conflict(ProblemDetailsFactory.CreateProblemDetails(
            "Conflict",
            StatusCodes.Status409Conflict,
            result.Error
        )),
        { Error.Type: ErrorType.Unauthorized } => Results.Unauthorized(),
        { Error.Type: ErrorType.Forbidden } => Results.Forbid(),
        _ => Results.BadRequest(ProblemDetailsFactory.CreateProblemDetails(
            "Bad Request",
            StatusCodes.Status400BadRequest,
            result.Error
        ))
    };
}