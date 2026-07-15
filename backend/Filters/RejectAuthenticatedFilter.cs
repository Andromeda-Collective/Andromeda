using Andromeda.Common.Factories;
using Andromeda.Features.Auth;

namespace Andromeda.Filters;

public sealed class RejectAuthenticatedFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            return Results.BadRequest(ProblemDetailsFactory.CreateProblemDetails(
                "Bad Request",
                StatusCodes.Status400BadRequest,
                AuthErrors.AlreadyAuthenticated
            ));
        }

        return await next(context);
    }
}