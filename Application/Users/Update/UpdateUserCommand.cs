using System.Text.RegularExpressions;
using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.Users.Common;
using Sanaclub.Domain.Auth;

namespace Sanaclub.Application.Users.Update;

public sealed class UpdateUserCommand : IRequest<UserResponse>
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string RoleCode { get; init; } = string.Empty;
    public Guid UpdatedByUserId { get; init; }
}

public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResponse>
{
    private const string UserIdField = "userId";
    private const string EmailField = "email";
    private const string FirstNameField = "firstName";
    private const string LastNameField = "lastName";
    private const string RoleCodeField = "roleCode";
    private const string UpdatedByUserIdField = "updatedByUserId";
    private const int MinNameLength = 1;

    private readonly IAuthRepository _authRepository;

    public UpdateUserCommandHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<UserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty)
        {
            throw new AppValidationException(
                UserIdField,
                "El identificador de usuario es obligatorio.");
        }

        if (request.UpdatedByUserId == Guid.Empty)
        {
            throw new AppValidationException(
                UpdatedByUserIdField,
                "El identificador del usuario actualizador es obligatorio.");
        }

        var email = request.Email.Trim();
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new AppValidationException(EmailField, "El correo electrónico es obligatorio.");
        }

        if (email.Length > UserManagementConstants.MaxEmailLength)
        {
            throw new AppValidationException(
                EmailField,
                $"El correo electrónico no puede superar {UserManagementConstants.MaxEmailLength} caracteres.");
        }

        if (!IsValidEmail(email))
        {
            throw new AppValidationException(EmailField, "El correo electrónico no es válido.");
        }

        var firstName = request.FirstName.Trim();
        var lastName = request.LastName.Trim();
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new AppValidationException(FirstNameField, "El nombre es obligatorio.");
        }

        if (firstName.Length < MinNameLength)
        {
            throw new AppValidationException(FirstNameField, "El nombre es obligatorio.");
        }

        if (firstName.Length > UserManagementConstants.MaxNameLength)
        {
            throw new AppValidationException(
                FirstNameField,
                $"El nombre no puede superar {UserManagementConstants.MaxNameLength} caracteres.");
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new AppValidationException(LastNameField, "El apellido es obligatorio.");
        }

        if (lastName.Length < MinNameLength)
        {
            throw new AppValidationException(LastNameField, "El apellido es obligatorio.");
        }

        if (lastName.Length > UserManagementConstants.MaxNameLength)
        {
            throw new AppValidationException(
                LastNameField,
                $"El apellido no puede superar {UserManagementConstants.MaxNameLength} caracteres.");
        }

        var roleCode = request.RoleCode.Trim();
        if (string.IsNullOrWhiteSpace(roleCode))
        {
            throw new AppValidationException(RoleCodeField, "El código del rol es obligatorio.");
        }

        if (!IsInternalRole(roleCode))
        {
            throw new AppValidationException(RoleCodeField, "El rol seleccionado no es válido para usuarios internos.");
        }

        var user = await _authRepository.GetUserByIdForUpdateAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("La persona usuaria seleccionada no existe.");
        }

        var normalizedEmail = email.ToLowerInvariant();
        var existingUserWithEmail = await _authRepository.GetUserByNormalizedEmailAsync(
            normalizedEmail,
            cancellationToken);

        if (existingUserWithEmail is not null && existingUserWithEmail.Id != request.UserId)
        {
            throw new ConflictException("Ya existe un usuario registrado con ese correo.");
        }

        var targetRole = await _authRepository.GetRoleByCodeAsync(roleCode, cancellationToken);
        if (targetRole is null || !targetRole.IsActive)
        {
            throw new AppValidationException(RoleCodeField, "El rol seleccionado no existe.");
        }

        await EnsureNotRemovingLastActiveAdminRole(request.UserId, targetRole, cancellationToken);

        var fullName = $"{firstName} {lastName}".Trim();
        if (fullName.Length > UserManagementConstants.MaxFullNameLength)
        {
            throw new AppValidationException(
                $"{FirstNameField},{LastNameField}",
                "El nombre completo no puede superar los 200 caracteres.");
        }

        user.ChangeEmail(email);
        user.UpdateFullName(fullName);
        user.MarkAsUpdated(request.UpdatedByUserId);

        var activeRoles = await _authRepository.GetActiveUserRolesByUserIdAsync(request.UserId, cancellationToken);
        foreach (var userRole in activeRoles)
        {
            if (userRole.RoleId == targetRole.Id)
            {
                continue;
            }

            if (!userRole.IsActive)
            {
                continue;
            }

            userRole.Revoke(
                request.UpdatedByUserId,
                UserManagementConstants.UsersUpdateRoleReason);
        }

        if (!activeRoles.Any(x => x.IsActive && x.RoleId == targetRole.Id))
        {
            var newRole = new UserRole(user.Id, targetRole.Id, request.UpdatedByUserId);
            newRole.MarkAsCreated(request.UpdatedByUserId);
            await _authRepository.AddUserRoleAsync(newRole, cancellationToken);
        }

        await _authRepository.SaveChangesAsync(cancellationToken);

        var roleForResponse = await _authRepository.GetActiveRoleByUserIdAsync(request.UserId, cancellationToken);
        return UserResponse.From(user, roleForResponse);
    }

    private async Task EnsureNotRemovingLastActiveAdminRole(Guid userId, Role targetRole, CancellationToken cancellationToken)
    {
        if (string.Equals(targetRole.Code, UserManagementConstants.AdminRoleCode, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var adminRole = await _authRepository.GetRoleByCodeAsync(UserManagementConstants.AdminRoleCode, cancellationToken);
        if (adminRole is null)
        {
            return;
        }

        var activeRoles = await _authRepository.GetActiveUserRolesByUserIdAsync(userId, cancellationToken);
        var userIsActiveAdmin = activeRoles.Any(x => x.IsActive && x.RoleId == adminRole.Id);
        if (!userIsActiveAdmin)
        {
            return;
        }

        var activeAdmins = await _authRepository.CountActiveUsersByRoleCodeAsync(
            UserManagementConstants.AdminRoleCode,
            cancellationToken);

        if (activeAdmins <= 1)
        {
            throw new ConflictException(UserManagementConstants.LastActiveAdminConflictMessage);
        }
    }

    private static bool IsInternalRole(string roleCode)
    {
        if (string.IsNullOrWhiteSpace(roleCode))
        {
            return false;
        }

        return UserManagementConstants.InternalRoleCodes
            .Any(code => string.Equals(code, roleCode.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsValidEmail(string email)
    {
        const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
    }
}
