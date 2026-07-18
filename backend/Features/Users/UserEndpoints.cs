using System.Security.Claims;
using Andromeda.Common;
using Andromeda.Common.Pagination;
using Andromeda.Extensions;
using Andromeda.Features.Users.DTOs;

namespace Andromeda.Features.Users;

public sealed class UserEndpoints : AbstractEndpoint
{
    public override void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/users").WithTags("Users");

        group.MapGet("", HandleGetUsers)
            .RequireAuthorization("AdminOrOwner")
            .WithName("GetUsers")
            .WithSummary("List users (paginated, filterable)")
            .WithDescription("Returns a paginated list of users. Filterable by username, email, role, and status. Requires Admin or Owner role.")
            .Produces<PagedResult<UserListItemResponse>>(StatusCodes.Status200OK);

        group.MapPost("", HandleCreateUser)
            .WithValidation<CreateUserRequest>()
            .RequireAuthorization("AdminOrOwner")
            .WithName("CreateUser")
            .WithSummary("Create a new user")
            .WithDescription("Owner can create Admin or User accounts. Admin can only create User accounts. Creating an Owner account is never allowed through this endpoint.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPatch("{id:guid}/status", HandleUpdateStatus)
            .WithValidation<UpdateUserStatusRequest>()
            .RequireAuthorization("AdminOrOwner")
            .WithName("UpdateUserStatus")
            .WithSummary("Change a user's status")
            .WithDescription("Admin can only change the status of plain Users. Owner can change the status of Admins or Users. Nobody can change the status of an Owner.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden);

        group.MapPut("{id:guid}", HandleUpdateUserByOwner)
            .WithValidation<UpdateUserByOwnerRequest>()
            .RequireAuthorization("OwnerOnly")
            .WithName("UpdateUserByOwner")
            .WithSummary("Fully edit another user's profile")
            .WithDescription("Owner-only. Updates first name, last name, username, and email for any Admin or User account. Cannot target an Owner account.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("{id:guid}/logout-all", HandleLogoutTarget)
            .RequireAuthorization("AdminOrOwner")
            .WithName("LogoutUserSessions")
            .WithSummary("Force logout all sessions of a target user")
            .WithDescription("Revokes every active refresh token for the target user. Admin cannot target other Admins. Nobody can target an Owner.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status403Forbidden);



        group.MapGet("me", HandleGetOwnProfile)
            .RequireAuthorization()
            .WithName("GetOwnProfile")
            .WithSummary("Get current user's profile")
            .WithDescription("Returns the authenticated user's own profile, including role, status, profile image, and active session count.")
            .Produces<UserProfileResponse>(StatusCodes.Status200OK);

        group.MapPut("me", HandleUpdateOwnProfile)
            .WithValidation<UpdateOwnProfileRequest>()
            .RequireAuthorization()
            .WithName("UpdateOwnProfile")
            .WithSummary("Update current user's first name, last name, and username")
            .WithDescription("The email address cannot be changed through this endpoint; only an Owner can change another user's email.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("me/password", HandleChangePassword)
            .WithValidation<ChangePasswordRequest>()
            .RequireAuthorization()
            .WithName("ChangePassword")
            .WithSummary("Change current user's password")
            .WithDescription("Requires the current password. On success, all active sessions (including the current one) are revoked, and the client must log in again.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPost("me/image", HandleUpdateProfileImage)
            .RequireAuthorization()
            .DisableAntiforgery()
            .WithName("UpdateProfileImage")
            .WithSummary("Upload or replace current user's profile image")
            .WithDescription("Accepts multipart/form-data with a single file field named 'file'. Allowed formats: jpg, jpeg, png, webp. Max size 2MB.")
            .Produces<string>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleGetUsers(
        [AsParameters] PaginationRequest pagination,
        string? username, string? email, Enums.Roles? role, Enums.UserState? state,
        IUserService userService)
    {
        var filter = new UserListFilter(username, email, role, state);
        var result = await userService.GetUsersAsync(pagination, filter);
        return result.IsSuccess ? Results.Ok(result.Value) : HandleFailure(result);
    }

    private static async Task<IResult> HandleCreateUser(
        CreateUserRequest request, ClaimsPrincipal caller, IUserService userService)
    {
        var result = await userService.CreateUserAsync(request, caller.GetRole());
        return result.IsSuccess ? Results.NoContent() : HandleFailure(result);
    }

    private static async Task<IResult> HandleUpdateStatus(
        Guid id, UpdateUserStatusRequest request, ClaimsPrincipal caller, IUserService userService)
    {
        var result = await userService.UpdateUserStatusAsync(id, caller.GetRole(), request);
        return result.IsSuccess ? Results.NoContent() : HandleFailure(result);
    }

    private static async Task<IResult> HandleUpdateUserByOwner(
        Guid id, UpdateUserByOwnerRequest request, ClaimsPrincipal caller, IUserService userService)
    {
        var result = await userService.UpdateUserByOwnerAsync(id, caller.GetRole(), request);
        return result.IsSuccess ? Results.NoContent() : HandleFailure(result);
    }

    private static async Task<IResult> HandleLogoutTarget(
        Guid id, ClaimsPrincipal caller, IUserService userService)
    {
        var result = await userService.LogoutAllSessionsForTargetAsync(id, caller.GetRole());
        return result.IsSuccess ? Results.NoContent() : HandleFailure(result);
    }

    private static async Task<IResult> HandleGetOwnProfile(ClaimsPrincipal caller, IUserService userService)
    {
        var result = await userService.GetProfileAsync(caller.GetUserId());
        return result.IsSuccess ? Results.Ok(result.Value) : HandleFailure(result);
    }

    private static async Task<IResult> HandleUpdateOwnProfile(
        UpdateOwnProfileRequest request, ClaimsPrincipal caller, IUserService userService)
    {
        var result = await userService.UpdateOwnProfileAsync(caller.GetUserId(), request);
        return result.IsSuccess ? Results.NoContent() : HandleFailure(result);
    }

    private static async Task<IResult> HandleChangePassword(
        ChangePasswordRequest request, ClaimsPrincipal caller, IUserService userService)
    {
        var result = await userService.ChangePasswordAsync(caller.GetUserId(), request);
        return result.IsSuccess ? Results.NoContent() : HandleFailure(result);
    }

    private static async Task<IResult> HandleUpdateProfileImage(
        IFormFile file, ClaimsPrincipal caller, IUserService userService)
    {
        var result = await userService.UpdateProfileImageAsync(caller.GetUserId(), file);
        return result.IsSuccess ? Results.Ok(result.Value) : HandleFailure(result);
    }
}