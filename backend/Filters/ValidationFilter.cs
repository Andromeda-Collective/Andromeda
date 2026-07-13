using Andromeda.Common;
using Andromeda.Common.Factories;
using FluentValidation;

namespace Andromeda.Filters;

public sealed class ValidationFilter<TRequest> : IEndpointFilter
    where TRequest : class
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().FirstOrDefault();

        if (request is null)
        {
            return await next(context);
        }

        var validator = context.HttpContext.RequestServices
            .GetService<IValidator<TRequest>>();

        if (validator is null)
        {
            return await next(context);
        }

        var validationResult = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);

        if (validationResult.IsValid)
        {
            return await next(context);
        }

        var errors = validationResult.Errors
            .Select(f => Error.Validation(f.PropertyName, f.ErrorMessage))
            .Distinct()
            .ToArray();

        return Results.BadRequest(ProblemDetailsFactory.CreateProblemDetails(
            "Validation Error",
            StatusCodes.Status400BadRequest,
            IValidationResult.ValidationError,
            errors
        ));
    }
}