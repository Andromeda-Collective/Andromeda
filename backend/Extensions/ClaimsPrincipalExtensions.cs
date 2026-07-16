using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Andromeda.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrEmpty(value) || !Guid.TryParse(value, out var userId))
        {
            throw new InvalidOperationException("User id claim is missing or invalid.");
        }

        return userId;
    }
}