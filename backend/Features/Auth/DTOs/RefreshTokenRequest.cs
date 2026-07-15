namespace Andromeda.Features.Auth.DTOs;

public sealed record RefreshTokenRequest(Guid UserId, string RefreshToken);