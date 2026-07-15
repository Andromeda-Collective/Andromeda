using Andromeda.Entities;

namespace Andromeda.Features.Auth;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRawRefreshToken();
    string HashRefreshToken(string rawRefreshToken);
}