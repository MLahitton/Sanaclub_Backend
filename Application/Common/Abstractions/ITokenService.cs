using Sanaclub.Application.Common.Security;
using Sanaclub.Domain.Auth;

namespace Sanaclub.Application.Common.Abstractions;

public interface ITokenService
{
    TokenResult GenerateTokens(
        User user,
        IReadOnlyCollection<string> roles,
        IReadOnlyCollection<string> permissions);

    string HashRefreshToken(string refreshToken);
}
