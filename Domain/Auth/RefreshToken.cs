using Sanaclub.Domain.Common;

namespace Sanaclub.Domain.Auth;

public sealed class RefreshToken : AuditableEntity
{
    private const int IpAddressMaxLength = 100;

    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public Guid? RevokedByUserId { get; private set; }
    public string? RevocationReason { get; private set; }
    public string? ReplacedByTokenHash { get; private set; }
    public string? CreatedByIpAddress { get; private set; }
    public string? RevokedByIpAddress { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
    public bool IsRevoked => RevokedAtUtc.HasValue;
    public bool IsActive => !IsRevoked && !IsExpired;

    private RefreshToken()
    {
    }

    private RefreshToken(
        Guid userId,
        string tokenHash,
        DateTime expiresAtUtc,
        string? createdByIpAddress)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
        CreatedByIpAddress = createdByIpAddress;
    }

    public static RefreshToken Create(
        Guid userId,
        string tokenHash,
        DateTime expiresAtUtc,
        string? createdByIpAddress = null,
        Guid? createdByUserId = null)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException("El identificador del usuario es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            throw new DomainException("El token hash es obligatorio.");
        }

        if (expiresAtUtc <= DateTime.UtcNow)
        {
            throw new DomainException("La fecha de expiración debe ser futura.");
        }

        if (!string.IsNullOrWhiteSpace(createdByIpAddress) && createdByIpAddress.Length > IpAddressMaxLength)
        {
            throw new DomainException("La dirección IP de creación supera el máximo permitido.");
        }

        var refreshToken = new RefreshToken(
            userId,
            tokenHash.Trim(),
            expiresAtUtc,
            string.IsNullOrWhiteSpace(createdByIpAddress) ? null : createdByIpAddress.Trim());

        refreshToken.MarkAsCreated(createdByUserId);
        return refreshToken;
    }

    public void Revoke(
        string reason,
        Guid? revokedByUserId = null,
        string? revokedByIpAddress = null,
        string? replacedByTokenHash = null)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new DomainException("La razón de revocación es obligatoria.");
        }

        if (IsRevoked)
        {
            throw new DomainException("El token refresh ya está revocado.");
        }

        if (!string.IsNullOrWhiteSpace(revokedByIpAddress) && revokedByIpAddress.Length > IpAddressMaxLength)
        {
            throw new DomainException("La dirección IP de revocación supera el máximo permitido.");
        }

        RevokedAtUtc = DateTime.UtcNow;
        RevokedByUserId = revokedByUserId;
        RevokedByIpAddress = string.IsNullOrWhiteSpace(revokedByIpAddress) ? null : revokedByIpAddress.Trim();
        RevocationReason = reason.Trim();
        ReplacedByTokenHash = string.IsNullOrWhiteSpace(replacedByTokenHash) ? null : replacedByTokenHash.Trim();
    }
}
