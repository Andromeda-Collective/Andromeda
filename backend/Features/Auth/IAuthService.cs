using Andromeda.Common;
using Andromeda.Features.Auth.DTOs;

namespace Andromeda.Features.Auth;

public interface IAuthService
{
    Task<Result> RegisterAsync(RegisterUserRequest request);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
    Task<Result<AuthResponse>> RefreshAsync(Guid userId, string rawRefreshToken);
    Task<Result> LogoutAsync(string rawRefreshToken, Guid requestingUserId);
}