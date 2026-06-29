using Sanaclub.Domain.Common;

namespace Sanaclub.Domain.Auth;

public sealed class User : AuditableEntity
{
    public string Email { get; private set; } = string.Empty;
    public string NormalizedEmail { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public bool MustChangePassword { get; private set; }
    public DateTime? LastLoginAtUtc { get; private set; }
    public DateTime? DeactivatedAtUtc { get; private set; }
    public string? DeactivationReason { get; private set; }

    private User()
    {
    }

    public User(string email, string passwordHash, string fullName, bool mustChangePassword = true)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("El correo es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new DomainException("El hash de la contraseña es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new DomainException("El nombre completo es obligatorio.");
        }

        Email = email.Trim();
        NormalizedEmail = Email.ToLowerInvariant();
        PasswordHash = passwordHash;
        FullName = fullName.Trim();
        IsActive = true;
        IsEmailVerified = false;
        MustChangePassword = mustChangePassword;
    }

    public void ChangeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("El correo es obligatorio.");
        }

        Email = email.Trim();
        NormalizedEmail = Email.ToLowerInvariant();
    }

    public void ChangePasswordHash(string passwordHash, bool mustChangePassword)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new DomainException("El hash de la contraseña es obligatorio.");
        }

        PasswordHash = passwordHash;
        MustChangePassword = mustChangePassword;
    }

    public void MarkEmailAsVerified()
    {
        IsEmailVerified = true;
    }

    public void MarkLogin(DateTime loggedAtUtc)
    {
        if (loggedAtUtc == default)
        {
            throw new DomainException("La fecha de login no es válida.");
        }

        LastLoginAtUtc = loggedAtUtc;
    }

    public void UpdateFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new DomainException("El nombre completo es obligatorio.");
        }

        var trimmedFullName = fullName.Trim();

        if (trimmedFullName.Length > 200)
        {
            throw new DomainException("El nombre completo no puede superar los 200 caracteres.");
        }

        FullName = trimmedFullName;
    }

    public void CompleteRequiredPasswordChange()
    {
        MustChangePassword = false;
    }

    public void Activate()
    {
        IsActive = true;
        DeactivatedAtUtc = null;
        DeactivationReason = null;
    }

    public void Deactivate(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new DomainException("La razón de desactivación es obligatoria.");
        }

        IsActive = false;
        DeactivatedAtUtc = DateTime.UtcNow;
        DeactivationReason = reason.Trim();
    }
}

