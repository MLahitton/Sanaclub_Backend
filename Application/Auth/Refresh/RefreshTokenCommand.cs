using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Domain.Auth;

namespace Sanaclub.Application.Auth.Refresh;

public sealed class RefreshTokenCommand : IRequest<RefreshTokenResponse>
{
    public string RefreshToken { get; init; } = string.Empty;
    public string? IpAddress { get; init; }
}

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private const string InvalidRefreshTokenMessage = "Invalid refresh token.";

    private readonly IAuthRepository _authRepository;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(
        IAuthRepository authRepository,
        ITokenService tokenService)
    {
        _authRepository = authRepository;
        _tokenService = tokenService;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            throw new ForbiddenException(InvalidRefreshTokenMessage);
        }

        var refreshTokenHash = _tokenService.HashRefreshToken(request.RefreshToken);
        var refreshToken = await _authRepository.GetRefreshTokenByHashAsync(refreshTokenHash, cancellationToken);

        if (refreshToken is null || !refreshToken.IsActive)
        {
            throw new ForbiddenException(InvalidRefreshTokenMessage);
        }

        var user = await _authRepository.GetUserByIdAsync(refreshToken.UserId, cancellationToken);

        if (user is null || !user.IsActive)
        {
            throw new ForbiddenException(InvalidRefreshTokenMessage);
        }

        var roles = await _authRepository.GetActiveRoleCodesByUserIdAsync(user.Id, cancellationToken);
        var permissions = await _authRepository.GetActivePermissionCodesByUserIdAsync(user.Id, cancellationToken);

        var tokenResult = _tokenService.GenerateTokens(user, roles, permissions);

        refreshToken.Revoke(
            reason: "Rotated",
            revokedByUserId: user.Id,
            revokedByIpAddress: request.IpAddress,
            replacedByTokenHash: tokenResult.RefreshTokenHash);

        var newRefreshToken = RefreshToken.Create(
            user.Id,
            tokenResult.RefreshTokenHash,
            tokenResult.RefreshTokenExpiresAtUtc,
            request.IpAddress,
            user.Id);

        await _authRepository.AddRefreshTokenAsync(newRefreshToken, cancellationToken);
        await _authRepository.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResponse
        {
            AccessToken = tokenResult.AccessToken,
            AccessTokenExpiresAtUtc = tokenResult.AccessTokenExpiresAtUtc,
            RefreshToken = tokenResult.RefreshToken,
            RefreshTokenExpiresAtUtc = tokenResult.RefreshTokenExpiresAtUtc
        };
    }
}
