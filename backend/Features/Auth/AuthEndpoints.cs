using System.Security.Claims;
using Andromeda.Common;
using Andromeda.Extensions;
using Andromeda.Features.Auth.DTOs;

namespace Andromeda.Features.Auth;

public sealed class AuthEndpoints : AbstractEndpoint
{
    public override void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth")
            .WithTags("Auth");

        group.MapPost("register", HandleRegister)
            .RejectIfAuthenticated()
            .WithValidation<RegisterUserRequest>()
            .WithName("Register")
            .WithSummary("Register a new user")
            .WithDescription("Creates a new user account with the 'User' role. Fails if the email or username is already taken, or if the caller is already authenticated.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("login", HandleLogin)
            .RejectIfAuthenticated()
            .WithValidation<LoginRequest>()
            .WithName("Login")
            .WithSummary("Authenticate a user")
            .WithDescription("Validates credentials and issues a new access/refresh token pair. Each successful login creates a new, independent session, so a user can be logged in from multiple devices at once.")
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        group.MapPost("refresh", HandleRefresh)
            .RejectIfAuthenticated()
            .WithValidation<RefreshTokenRequest>()
            .WithName("Refresh")
            .WithSummary("Rotate an access/refresh token pair")
            .WithDescription("Exchanges a valid, non-expired refresh token for a new access/refresh token pair. The existing session record is rotated in place rather than creating a new one. Does not require an access token, since the current one may already be expired.")
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPost("logout", HandleLogout)
            .RequireAuthorization()
            .WithValidation<LogoutRequest>()
            .WithName("Logout")
            .WithSummary("Revoke a single session")
            .WithDescription("Revokes the refresh token for the current device only. Requires a valid access token; the refresh token being revoked must belong to the authenticated caller, otherwise the request is rejected as invalid.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> HandleRegister(
        RegisterUserRequest request,
        IAuthService authService,
        CancellationToken ct)
    {
        var result = await authService.RegisterAsync(request, ct);

        return result.IsSuccess
            ? Results.NoContent()
            : HandleFailure(result);
    }

    private static async Task<IResult> HandleLogin(
        LoginRequest request,
        IAuthService authService,
        CancellationToken ct)
    {
        var result = await authService.LoginAsync(request, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : HandleFailure(result);
    }

    private static async Task<IResult> HandleRefresh(
        RefreshTokenRequest request,
        IAuthService authService,
        CancellationToken ct)
    {
        var result = await authService.RefreshAsync(request.UserId, request.RefreshToken, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : HandleFailure(result);
    }

    private static async Task<IResult> HandleLogout(
        LogoutRequest request,
        ClaimsPrincipal user,
        IAuthService authService,
        CancellationToken ct)
    {
        var userId = user.GetUserId();

        var result = await authService.LogoutAsync(request.RefreshToken, userId, ct);

        return result.IsSuccess
            ? Results.NoContent()
            : HandleFailure(result);
    }
}