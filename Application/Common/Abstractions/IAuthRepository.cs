using Sanaclub.Domain.Auth;

namespace Sanaclub.Application.Common.Abstractions;

public interface IAuthRepository
{
    Task<User?> GetUserByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default);

    Task<User?> GetUserByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<User?> GetUserByIdForUpdateAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<string>> GetActiveRoleCodesByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<string>> GetActivePermissionCodesByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<RefreshToken?> GetRefreshTokenByHashAsync(
        string refreshTokenHash,
        CancellationToken cancellationToken = default);

    Task AddRefreshTokenAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
