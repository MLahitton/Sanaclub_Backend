using System.Text.RegularExpressions;
using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.Users.Common;
using Sanaclub.Domain.Auth;

namespace Sanaclub.Application.Users.Create;

public sealed class CreateUserCommand : IRequest<UserResponse>
{
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string RoleCode { get; init; } = string.Empty;
    public string TemporaryPassword { get; init; } = string.Empty;
    public Guid CreatedByUserId { get; init; }
}

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponse>
{
    private const string EmailField = "email";
    private const string FirstNameField = "firstName";
    private const string LastNameField = "lastName";
    private const string RoleCodeField = "roleCode";
    private const string TemporaryPasswordField = "temporaryPassword";
    private const int MinNameLength = 1;

    private readonly IAuthRepository _authRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(
        IAuthRepository authRepository,
        IPasswordHasher passwordHasher)
    {
        _authRepository = authRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (request.CreatedByUserId == Guid.Empty)
        {
            throw new AppValidationException(
                "createdByUserId",
                "El identificador del usuario creador es obligatorio.");
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

        var temporaryPassword = request.TemporaryPassword;
        if (string.IsNullOrWhiteSpace(temporaryPassword))
        {
            throw new AppValidationException(TemporaryPasswordField, "La contraseña temporal es obligatoria.");
        }

        if (!IsStrongPassword(temporaryPassword))
        {
            throw new AppValidationException(
                TemporaryPasswordField,
                "La contraseña temporal no cumple con los requisitos mínimos de seguridad.");
        }

        var normalizedEmail = email.ToLowerInvariant();
        var userExists = await _authRepository.GetUserByNormalizedEmailAsync(normalizedEmail, cancellationToken);
        if (userExists is not null)
        {
            throw new ConflictException("Ya existe un usuario con ese correo electrónico.");
        }

        var role = await _authRepository.GetRoleByCodeAsync(roleCode, cancellationToken);
        if (role is null || !role.IsActive)
        {
            throw new AppValidationException(RoleCodeField, "El rol seleccionado no existe.");
        }

        var fullName = $"{firstName} {lastName}".Trim();
        if (fullName.Length > UserManagementConstants.MaxFullNameLength)
        {
            throw new AppValidationException(
                $"{FirstNameField},{LastNameField}",
                "El nombre completo no puede superar los 200 caracteres.");
        }

        var temporaryPasswordHash = _passwordHasher.Hash(temporaryPassword);
        var user = new User(email, temporaryPasswordHash, fullName);
        user.MarkAsCreated(request.CreatedByUserId);

        var userRole = new UserRole(user.Id, role.Id, request.CreatedByUserId);
        userRole.MarkAsCreated(request.CreatedByUserId);

        await _authRepository.AddUserAsync(user, cancellationToken);
        await _authRepository.AddUserRoleAsync(userRole, cancellationToken);
        await _authRepository.SaveChangesAsync(cancellationToken);

        return UserResponse.From(user, role);
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

