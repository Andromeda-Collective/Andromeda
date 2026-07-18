using Andromeda.Common;
using Andromeda.Common.Errors;
using Andromeda.Common.Pagination;
using Andromeda.Common.Services.Cache;
using Andromeda.Common.Services.FileStorage;
using Andromeda.Data;
using Andromeda.Entities;
using Andromeda.Enums;
using Andromeda.Features.Users.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Andromeda.Features.Users;

public sealed class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _dbContext;
    private readonly IUserCacheService _userCache;
    private readonly IFileStorageService _fileStorage;

    public UserService(
        UserManager<User> userManager,
        ApplicationDbContext dbContext,
        IUserCacheService userCache,
        IFileStorageService fileStorage)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _userCache = userCache;
        _fileStorage = fileStorage;
    }

    public async Task<Result<PagedResult<UserListItemResponse>>> GetUsersAsync(
        PaginationRequest pagination, UserListFilter filter, CancellationToken ct = default)
    {
        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Username))
            query = query.Where(u => u.UserName!.Contains(filter.Username));

        if (!string.IsNullOrWhiteSpace(filter.Email))
            query = query.Where(u => u.Email!.Contains(filter.Email));

        if (filter.State.HasValue)
            query = query.Where(u => u.State == filter.State.Value);

        if (filter.Role.HasValue)
        {
            var roleName = filter.Role.Value.ToString();
            var userIdsInRole = _dbContext.UserRoles
                .Join(_dbContext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
                .Where(x => x.Name == roleName)
                .Select(x => x.UserId);

            query = query.Where(u => userIdsInRole.Contains(u.Id));
        }

        query = query.OrderBy(u => u.UserName);

        var paged = await PagedResult<User>.CreateAsync(query, pagination.Page, pagination.PageSize, ct);

        var items = new List<UserListItemResponse>(paged.Items.Count);
        foreach (var user in paged.Items)
        {
            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? nameof(Roles.User);
            var sessionCount = await ActiveSessionCountAsync(user.Id, ct);

            items.Add(new UserListItemResponse(
                user.Id, user.UserName!, user.Email!, user.FirstName, user.LastName,
                role, user.State, user.ProfileImagePath, sessionCount));
        }

        var result = new PagedResult<UserListItemResponse>(
            items, paged.Page, paged.PageSize, paged.TotalCount);

        return Result.Success(result);
    }

    public async Task<Result<UserProfileResponse>> GetProfileAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return UserErrors.UserNotFound;

        var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? nameof(Roles.User);
        var sessionCount = await ActiveSessionCountAsync(user.Id, ct);

        return Result.Success(new UserProfileResponse(
            user.Id, user.UserName!, user.Email!, user.FirstName, user.LastName,
            role, user.State, user.ProfileImagePath, sessionCount));
    }

    public async Task<Result> CreateUserAsync(CreateUserRequest request, string callerRole, CancellationToken ct = default)
    {
        if (!RoleHierarchy.CanCreateWithRole(callerRole, request.Role))
            return UserErrors.CannotAssignRole;

        if (await _userManager.FindByEmailAsync(request.Email) is not null)
            return CommonErrors.EmailAlreadyExists;

        if (await _userManager.FindByNameAsync(request.Username) is not null)
            return CommonErrors.UsernameAlreadyExists;

        var user = new User
        {
            UserName = request.Username,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            State = UserState.Active,
            ProfileImagePath = ProfileImageDefaults.DefaultImagePath
        };

        var identityResult = await _userManager.CreateAsync(user, request.Password);
        if (!identityResult.Succeeded)
            return CommonErrors.RegistrationFailed;

        await _userManager.AddToRoleAsync(user, request.Role.ToString());

        return Result.Success();
    }

    public async Task<Result> UpdateOwnProfileAsync(Guid userId, UpdateOwnProfileRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return UserErrors.UserNotFound;

        var usernameOwner = await _userManager.FindByNameAsync(request.Username);
        if (usernameOwner is not null && usernameOwner.Id != user.Id)
            return CommonErrors.UsernameAlreadyExists;

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.UserName = request.Username;
        user.NormalizedUserName = _userManager.NormalizeName(request.Username);

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return UserErrors.UpdateFailed;

        return Result.Success();
    }

    public async Task<Result> UpdateUserByOwnerAsync(
        Guid targetUserId, string callerRole, UpdateUserByOwnerRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(targetUserId.ToString());
        if (user is null)
            return UserErrors.UserNotFound;

        var targetRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? nameof(Roles.User);

        if (!RoleHierarchy.CanEditFullProfile(callerRole, targetRole))
            return UserErrors.CannotModifyOwner;

        var usernameOwner = await _userManager.FindByNameAsync(request.Username);
        if (usernameOwner is not null && usernameOwner.Id != user.Id)
            return CommonErrors.UsernameAlreadyExists;

        var emailOwner = await _userManager.FindByEmailAsync(request.Email);
        if (emailOwner is not null && emailOwner.Id != user.Id)
            return CommonErrors.EmailAlreadyExists;

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.UserName = request.Username;
        user.NormalizedUserName = _userManager.NormalizeName(request.Username);
        user.Email = request.Email;
        user.NormalizedEmail = _userManager.NormalizeEmail(request.Email);

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return UserErrors.UpdateFailed;

        return Result.Success();
    }

    public async Task<Result> UpdateUserStatusAsync(
        Guid targetUserId, string callerRole, UpdateUserStatusRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(targetUserId.ToString());
        if (user is null)
            return UserErrors.UserNotFound;

        var targetRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? nameof(Roles.User);

        if (!RoleHierarchy.CanChangeStatus(callerRole, targetRole))
            return UserErrors.CannotModifyOwner;

        user.State = request.State;
        await _userManager.UpdateAsync(user);


        await _userCache.InvalidateAsync(user.Id, ct);

        if (request.State == UserState.Banned)
        {
            var tokens = await _dbContext.Tokens
                .Where(t => t.UserId == user.Id && !t.IsRevoked)
                .ToListAsync(ct);

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync(ct);
        }

        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return UserErrors.UserNotFound;

        var changeResult = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!changeResult.Succeeded)
            return UserErrors.PasswordChangeFailed;

        var tokens = await _dbContext.Tokens
            .Where(t => t.UserId == userId && !t.IsRevoked)
            .ToListAsync(ct);

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<Result<string>> UpdateProfileImageAsync(Guid userId, IFormFile file, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return UserErrors.UserNotFound;


        var saveProfileResult = await _fileStorage.SaveProfileImageAsync(userId, file, ct);
        if (saveProfileResult.IsFailure)
            return saveProfileResult.Error;

        var newPath = saveProfileResult.Value;

        var oldPath = user.ProfileImagePath;
        user.ProfileImagePath = newPath;
        await _userManager.UpdateAsync(user);

        _fileStorage.DeleteProfileImage(oldPath);

        return Result.Success(newPath);
    }

    public async Task<Result> LogoutAllSessionsForTargetAsync(
        Guid targetUserId, string callerRole, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(targetUserId.ToString());
        if (user is null)
            return UserErrors.UserNotFound;

        var targetRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? nameof(Roles.User);

        if (!RoleHierarchy.CanLogoutTarget(callerRole, targetRole))
            return UserErrors.CannotLogoutTarget;

        var tokens = await _dbContext.Tokens
            .Where(t => t.UserId == targetUserId && !t.IsRevoked)
            .ToListAsync(ct);

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync(ct);

        return Result.Success();
    }

    private async Task<int> ActiveSessionCountAsync(Guid userId, CancellationToken ct)
        => await _dbContext.Tokens.CountAsync(
            t => t.UserId == userId && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow, ct);
}