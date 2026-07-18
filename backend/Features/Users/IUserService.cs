using Andromeda.Common;
using Andromeda.Common.Pagination;
using Andromeda.Features.Users.DTOs;

namespace Andromeda.Features.Users;

public interface IUserService
{
    Task<Result<PagedResult<UserListItemResponse>>> GetUsersAsync(
        PaginationRequest pagination, UserListFilter filter, CancellationToken ct = default);

    Task<Result<UserProfileResponse>> GetProfileAsync(Guid userId, CancellationToken ct = default);

    Task<Result> CreateUserAsync(CreateUserRequest request, string callerRole, CancellationToken ct = default);

    Task<Result> UpdateOwnProfileAsync(Guid userId, UpdateOwnProfileRequest request, CancellationToken ct = default);

    Task<Result> UpdateUserByOwnerAsync(
        Guid targetUserId, string callerRole, UpdateUserByOwnerRequest request, CancellationToken ct = default);

    Task<Result> UpdateUserStatusAsync(
        Guid targetUserId, string callerRole, UpdateUserStatusRequest request, CancellationToken ct = default);

    Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken ct = default);

    Task<Result<string>> UpdateProfileImageAsync(Guid userId, IFormFile file, CancellationToken ct = default);

    Task<Result> LogoutAllSessionsForTargetAsync(
        Guid targetUserId, string callerRole, CancellationToken ct = default);
}