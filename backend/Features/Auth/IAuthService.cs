using Andromeda.Common;
using Andromeda.Features.Auth.DTOs;

namespace Andromeda.Features.Auth;

public interface IAuthService
{
    Task<Result> RegisterAsync(RegisterUserRequest request, CancellationToken ct = default);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<Result<AuthResponse>> RefreshAsync(Guid userId, string rawRefreshToken, CancellationToken ct = default);
    Task<Result> LogoutAsync(string rawRefreshToken, Guid requestingUserId, CancellationToken ct = default);
}