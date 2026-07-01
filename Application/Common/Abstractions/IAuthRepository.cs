using Sanaclub.Domain.Auth;
using Sanaclub.Application.Appointments.ListTherapists;

namespace Sanaclub.Application.Common.Abstractions;

public interface IAuthRepository
{
    Task AddUserAsync(
        User user,
        CancellationToken cancellationToken = default);

    Task<User?> GetUserByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<User>> ListUsersAsync(
        string? search,
        bool? isActive,
        string? roleCode,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<int> CountUsersAsync(
        string? search,
        bool? isActive,
        string? roleCode,
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

    Task<IReadOnlyCollection<AppointmentTherapistResponse>> ListActiveTherapistsAsync(
        CancellationToken cancellationToken = default);

    Task<Role?> GetRoleByCodeAsync(
        string roleCode,
        CancellationToken cancellationToken = default);

    Task<Role?> GetActiveRoleByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UserRole>> GetActiveUserRolesByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<int> CountActiveUsersByRoleCodeAsync(
        string roleCode,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RefreshToken>> GetActiveRefreshTokensByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task AddUserRoleAsync(
        UserRole userRole,
        CancellationToken cancellationToken = default);

    Task<RefreshToken?> GetRefreshTokenByHashAsync(
        string refreshTokenHash,
        CancellationToken cancellationToken = default);

    Task AddRefreshTokenAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
