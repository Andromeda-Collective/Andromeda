using Andromeda.Common;
using Andromeda.Common.Errors;
using Andromeda.Common.Services.FileStorage;
using Andromeda.Data;
using Andromeda.Entities;
using Andromeda.Enums;
using Andromeda.Features.Auth.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Andromeda.Features.Auth;



public sealed class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ApplicationDbContext _dbContext;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ITokenService tokenService,
        ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _dbContext = dbContext;
    }

    public async Task<Result> RegisterAsync(RegisterUserRequest request, CancellationToken ct = default)
    {
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

        await _userManager.AddToRoleAsync(user, Roles.User.ToString());

        return Result.Success();
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return AuthErrors.InvalidCredentials;

        if (user.State != UserState.Active)
            return AuthErrors.UserNotActive;

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!signInResult.Succeeded)
        {
            return signInResult.IsLockedOut
                ? AuthErrors.UserLockedOut
                : AuthErrors.InvalidCredentials;
        }


        var authResponse = await IssueNewSessionAsync(user, ct);

        return Result.Success(authResponse);
    }

    public async Task<Result<AuthResponse>> RefreshAsync(Guid userId, string rawRefreshToken, CancellationToken ct = default)
    {
        var hashed = _tokenService.HashRefreshToken(rawRefreshToken);

        var token = await _dbContext.Tokens
            .FirstOrDefaultAsync(t => t.TokenValue == hashed, ct);

        if (token is null || token.UserId != userId)
            return AuthErrors.InvalidRefreshToken;

        if (token.IsRevoked)
            return AuthErrors.RefreshTokenRevoked;

        if (token.ExpiresAt < DateTime.UtcNow)
            return AuthErrors.RefreshTokenExpired;

        var user = await _userManager.FindByIdAsync(token.UserId.ToString());
        if (user is null)
            return AuthErrors.UserNotFound;

        if (user.State != UserState.Active)
            return AuthErrors.UserNotActive;

        var rawNewToken = _tokenService.GenerateRawRefreshToken();
        token.TokenValue = _tokenService.HashRefreshToken(rawNewToken);
        token.ExpiresAt = DateTime.UtcNow.AddDays(30);
        token.UpdatedAt = DateTime.UtcNow;

        var roles = await _userManager.GetRolesAsync(user);

        var accessToken = _tokenService.GenerateAccessToken(user, roles);

        await _dbContext.SaveChangesAsync(ct);

        return Result.Success(new AuthResponse(
            UserId: user.Id,
            AccessToken: accessToken,
            RefreshToken: rawNewToken,
            ExpiresAt: token.ExpiresAt,
            Username: user.UserName!,
            Email: user.Email!,
            Role: roles.First(),
            ProfileImageUrl: user.ProfileImagePath
        ));
    }

    public async Task<Result> LogoutAsync(string rawRefreshToken, Guid requestingUserId, CancellationToken ct = default)
    {
        var hashed = _tokenService.HashRefreshToken(rawRefreshToken);

        var token = await _dbContext.Tokens
            .FirstOrDefaultAsync(t => t.TokenValue == hashed && !t.IsRevoked, ct);


        if (token is null || token.UserId != requestingUserId)
        {
            return AuthErrors.InvalidRefreshToken;
        }

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(ct);

        return Result.Success();
    }

    private async Task<AuthResponse> IssueNewSessionAsync(User user, CancellationToken ct = default)
    {
        var rawRefreshToken = _tokenService.GenerateRawRefreshToken();

        var token = new Token
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenValue = _tokenService.HashRefreshToken(rawRefreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        _dbContext.Tokens.Add(token);
        await _dbContext.SaveChangesAsync(ct);

        var roles = await _userManager.GetRolesAsync(user);

        return new AuthResponse(
            UserId: user.Id,
            AccessToken: _tokenService.GenerateAccessToken(user, roles),
            RefreshToken: rawRefreshToken,
            ExpiresAt: token.ExpiresAt,
            Username: user.UserName!,
            Email: user.Email!,
            Role: roles.First(),
            ProfileImageUrl: user.ProfileImagePath
        );
    }
}