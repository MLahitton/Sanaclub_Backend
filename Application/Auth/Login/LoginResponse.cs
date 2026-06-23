using Sanaclub.Application.Common.Security;
using Sanaclub.Domain.Auth;

namespace Sanaclub.Application.Auth.Login;

public sealed class LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public DateTime AccessTokenExpiresAtUtc { get; init; }
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime RefreshTokenExpiresAtUtc { get; init; }
    public AuthenticatedUserResponse User { get; init; } = new();
}
