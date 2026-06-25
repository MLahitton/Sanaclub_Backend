using MediatR;
using Sanaclub.Application.Common.Abstractions;

namespace Sanaclub.Application.Auth.Logout;

public sealed class LogoutCommand : IRequest<Unit>
{
    public string RefreshToken { get; init; } = string.Empty;
    public string? IpAddress { get; init; }
}

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IAuthRepository _authRepository;
    private readonly ITokenService _tokenService;

    public LogoutCommandHandler(
        IAuthRepository authRepository,
        ITokenService tokenService)
    {
        _authRepository = authRepository;
        _tokenService = tokenService;
    }

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return Unit.Value;
        }

        var refreshTokenHash = _tokenService.HashRefreshToken(request.RefreshToken);
        var refreshToken = await _authRepository.GetRefreshTokenByHashAsync(refreshTokenHash, cancellationToken);

        if (refreshToken is null || !refreshToken.IsActive)
        {
            return Unit.Value;
        }

        refreshToken.Revoke(
            reason: "Logout",
            revokedByUserId: refreshToken.UserId,
            revokedByIpAddress: request.IpAddress,
            replacedByTokenHash: null);

        await _authRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
