namespace Andromeda.Features.Auth.DTOs;

public sealed record AuthResponse(
    Guid UserId,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    string Username,
    string Email,
    string Role,
    string ProfileImageUrl
);