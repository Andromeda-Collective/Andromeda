using System.Reflection;
using Andromeda.Common;

namespace Andromeda.Extensions;

public static class EndpointExtensions
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpointType = typeof(AbstractEndpoint);

        var endpoints = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(x =>
                endpointType.IsAssignableFrom(x) &&
                !x.IsInterface &&
                !x.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<AbstractEndpoint>();

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(app);
        }
    }
}