namespace Sanaclub.Application.Common.Security;

public sealed class TokenResult
{
    public string AccessToken { get; init; } = string.Empty;
    public DateTime AccessTokenExpiresAtUtc { get; init; }
    public string RefreshToken { get; init; } = string.Empty;
    public string RefreshTokenHash { get; init; } = string.Empty;
    public DateTime RefreshTokenExpiresAtUtc { get; init; }
}
