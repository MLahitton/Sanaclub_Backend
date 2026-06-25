using System.Text.RegularExpressions;
using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;

namespace Sanaclub.Application.Auth.ChangePassword;

public sealed class ChangePasswordCommand : IRequest<Unit>
{
    public Guid UserId { get; init; }
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
    public string ConfirmNewPassword { get; init; } = string.Empty;
}

public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Unit>
{
    private const string GenericMessage = "Invalid password change request.";
    private const string NewPasswordField = "newPassword";

    private readonly IAuthRepository _authRepository;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(
        IAuthRepository authRepository,
        IPasswordHasher passwordHasher)
    {
        _authRepository = authRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty)
        {
            throw new ForbiddenException(GenericMessage);
        }

        if (string.IsNullOrWhiteSpace(request.CurrentPassword))
        {
            throw new ForbiddenException(GenericMessage);
        }

        if (string.IsNullOrWhiteSpace(request.NewPassword))
        {
            throw new ForbiddenException(GenericMessage);
        }

        if (string.IsNullOrWhiteSpace(request.ConfirmNewPassword))
        {
            throw new ForbiddenException(GenericMessage);
        }

        if (request.NewPassword != request.ConfirmNewPassword)
        {
            throw new AppValidationException(
                "confirmNewPassword",
                "The confirmation password does not match the new password.");
        }

        if (!IsStrongPassword(request.NewPassword))
        {
            throw new AppValidationException(
                NewPasswordField,
                "The new password does not meet minimum security requirements.");
        }

        var user = await _authRepository.GetUserByIdForUpdateAsync(request.UserId, cancellationToken);

        if (user is null || !user.IsActive)
        {
            throw new ForbiddenException(GenericMessage);
        }

        var currentPasswordMatch = _passwordHasher.Verify(request.CurrentPassword, user.PasswordHash);

        if (!currentPasswordMatch)
        {
            throw new ForbiddenException(GenericMessage);
        }

        if (_passwordHasher.Verify(request.NewPassword, user.PasswordHash))
        {
            throw new AppValidationException(
                NewPasswordField,
                "The new password must be different from the current password.");
        }

        var newPasswordHash = _passwordHasher.Hash(request.NewPassword);

        user.ChangePasswordHash(newPasswordHash, mustChangePassword: false);
        user.CompleteRequiredPasswordChange();

        await _authRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private static bool IsStrongPassword(string password)
    {
        if (password.Length < 8)
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
