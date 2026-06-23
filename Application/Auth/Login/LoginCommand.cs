using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Domain.Auth;

namespace Sanaclub.Application.Auth.Login;

public sealed class LoginCommand : IRequest<LoginResponse>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string? IpAddress { get; init; }
}

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private const string InvalidCredentialsMessage = "Invalid email or password.";

    private readonly IAuthRepository _authRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        IAuthRepository authRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _authRepository = authRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ForbiddenException(InvalidCredentialsMessage);
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ForbiddenException(InvalidCredentialsMessage);
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await _authRepository.GetUserByNormalizedEmailAsync(normalizedEmail, cancellationToken);

        if (user is null || !user.IsActive)
        {
            throw new ForbiddenException(InvalidCredentialsMessage);
        }

        var isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            throw new ForbiddenException(InvalidCredentialsMessage);
        }

        var roles = await _authRepository.GetActiveRoleCodesByUserIdAsync(user.Id, cancellationToken);
        var permissions = await _authRepository.GetActivePermissionCodesByUserIdAsync(user.Id, cancellationToken);

        var tokenResult = _tokenService.GenerateTokens(user, roles, permissions);

        var refreshToken = RefreshToken.Create(
            user.Id,
            tokenResult.RefreshTokenHash,
            tokenResult.RefreshTokenExpiresAtUtc,
            request.IpAddress,
            user.Id);

        await _authRepository.AddRefreshTokenAsync(refreshToken, cancellationToken);
        await _authRepository.SaveChangesAsync(cancellationToken);

        return new LoginResponse
        {
            AccessToken = tokenResult.AccessToken,
            AccessTokenExpiresAtUtc = tokenResult.AccessTokenExpiresAtUtc,
            RefreshToken = tokenResult.RefreshToken,
            RefreshTokenExpiresAtUtc = tokenResult.RefreshTokenExpiresAtUtc,
            User = new AuthenticatedUserResponse
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                MustChangePassword = user.MustChangePassword,
                Roles = roles,
                Permissions = permissions
            }
        };
    }
}
