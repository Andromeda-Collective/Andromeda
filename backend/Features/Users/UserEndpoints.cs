using Andromeda.Common;

namespace Andromeda.Features.Users;


public sealed class UserEndpoints : AbstractEndpoint
{
    public override void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/users")
            .WithTags("Users");
        
        group.MapGet("", () =>
        {
            return Results.Ok();
        });
    }
}