using System.Text.RegularExpressions;
using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.Users.Common;

namespace Sanaclub.Application.Users.ResetPassword;

public sealed class ResetUserPasswordCommand : IRequest<Unit>
{
    public Guid UserId { get; init; }
    public string NewTemporaryPassword { get; init; } = string.Empty;
    public Guid ResetByUserId { get; init; }
}

public sealed class ResetUserPasswordCommandHandler : IRequestHandler<ResetUserPasswordCommand, Unit>
{
    private const string UserIdField = "userId";
    private const string PasswordField = "newTemporaryPassword";
    private const string ResetByUserIdField = "resetByUserId";

    private readonly IAuthRepository _authRepository;
    private readonly IPasswordHasher _passwordHasher;

    public ResetUserPasswordCommandHandler(
        IAuthRepository authRepository,
        IPasswordHasher passwordHasher)
    {
        _authRepository = authRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Unit> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty)
        {
            throw new AppValidationException(UserIdField, "El identificador de usuario es obligatorio.");
        }

        if (request.ResetByUserId == Guid.Empty)
        {
            throw new AppValidationException(
                ResetByUserIdField,
                "El identificador del usuario que resetea es obligatorio.");
        }

        var temporaryPassword = request.NewTemporaryPassword;
        if (string.IsNullOrWhiteSpace(temporaryPassword))
        {
            throw new AppValidationException(PasswordField, "La contraseña temporal es obligatoria.");
        }

        if (!IsStrongPassword(temporaryPassword))
        {
            throw new AppValidationException(
                PasswordField,
                "La contraseña temporal no cumple con los requisitos mínimos de seguridad.");
        }

        var user = await _authRepository.GetUserByIdForUpdateAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("El usuario no fue encontrado.");
        }

        if (_passwordHasher.Verify(temporaryPassword, user.PasswordHash))
        {
            throw new AppValidationException(PasswordField, "La nueva contraseña debe ser distinta de la actual.");
        }

        var passwordHash = _passwordHasher.Hash(temporaryPassword);
        user.ChangePasswordHash(passwordHash, mustChangePassword: true);
        user.MarkAsUpdated(request.ResetByUserId);

        var activeRefreshTokens = await _authRepository.GetActiveRefreshTokensByUserIdAsync(
            request.UserId,
            cancellationToken);

        foreach (var refreshToken in activeRefreshTokens)
        {
            refreshToken.Revoke(
                reason: UserManagementConstants.UsersResetPasswordReason,
                revokedByUserId: request.ResetByUserId);
        }

        await _authRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private static bool IsStrongPassword(string password)
    {
        if (password.Length < UserManagementConstants.MinPasswordLength)
        {
            return false;
        }

        var hasUpper = Regex.IsMatch(password, "[A-Z]");
        var hasLower = Regex.IsMatch(password, "[a-z]");
        var hasDigit = Regex.IsMatch(password, "\\d");
        var hasSpecial = Regex.IsMatch(password, "[^a-zA-Z0-9]");

        return hasUpper && hasLower && hasDigit && hasSpecial;
    }
}
