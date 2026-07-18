using Andromeda.Entities;

namespace Andromeda.Features.Auth;

public interface ITokenService
{
    string GenerateAccessToken(User user, IList<string> roles);
    string GenerateRawRefreshToken();
    string HashRefreshToken(string rawRefreshToken);
}