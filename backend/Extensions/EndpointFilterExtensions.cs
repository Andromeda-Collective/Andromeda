using Andromeda.Filters;

namespace Andromeda.Extensions;

public static class EndpointFilterExtensions
{
    public static RouteHandlerBuilder WithValidation<TRequest>(
        this RouteHandlerBuilder builder)
        where TRequest : class
        => builder.AddEndpointFilter<ValidationFilter<TRequest>>();
}