using Microsoft.EntityFrameworkCore;
using Sanaclub.Application.Appointments.ListTherapists;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Domain.Auth;
using Sanaclub.Infrastructure.Persistence;

namespace Sanaclub.Infrastructure.Persistence.Repositories;

public sealed class AuthRepository : IAuthRepository
{
    private readonly SanaclubDbContext _context;

    public AuthRepository(SanaclubDbContext context)
    {
        _context = context;
    }

    public async Task AddUserAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public async Task AddUserRoleAsync(
        UserRole userRole,
        CancellationToken cancellationToken = default)
    {
        await _context.UserRoles.AddAsync(userRole, cancellationToken);
    }

    public async Task<IReadOnlyCollection<User>> ListUsersAsync(
        string? search,
        bool? isActive,
        string? roleCode,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = BuildUsersSearchQuery(_context.Users.AsNoTracking(), search);

        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(roleCode))
        {
            var normalizedRoleCode = roleCode.Trim().ToUpperInvariant();

            query = query.Where(user => _context.UserRoles.Any(
                userRole => userRole.UserId == user.Id
                          && userRole.IsActive
                          && _context.Roles.Any(role => role.Id == userRole.RoleId
                                                        && role.IsActive
                                                        && role.Code == normalizedRoleCode)));
        }

        return await query
            .OrderBy(x => x.FullName)
            .ThenBy(x => x.Email)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountUsersAsync(
        string? search,
        bool? isActive,
        string? roleCode,
        CancellationToken cancellationToken = default)
    {
        var query = BuildUsersSearchQuery(_context.Users.AsNoTracking(), search);

        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(roleCode))
        {
            var normalizedRoleCode = roleCode.Trim().ToUpperInvariant();

            query = query.Where(user => _context.UserRoles.Any(
                userRole => userRole.UserId == user.Id
                          && userRole.IsActive
                          && _context.Roles.Any(role => role.Id == userRole.RoleId
                                                        && role.IsActive
                                                        && role.Code == normalizedRoleCode)));
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<int> CountActiveUsersByRoleCodeAsync(
        string roleCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(roleCode))
        {
            return 0;
        }

        var normalizedRoleCode = roleCode.Trim().ToUpperInvariant();

        var query = from user in _context.Users
                    join userRole in _context.UserRoles on user.Id equals userRole.UserId
                    join role in _context.Roles on userRole.RoleId equals role.Id
                    where user.IsActive
                          && userRole.IsActive
                          && role.IsActive
                          && role.Code == normalizedRoleCode
                    select user.Id;

        return await query.Distinct().CountAsync(cancellationToken);
    }

    public async Task<Role?> GetRoleByCodeAsync(
        string roleCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(roleCode))
        {
            return null;
        }

        var normalizedRoleCode = roleCode.Trim().ToUpperInvariant();

        return await _context.Roles
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Code == normalizedRoleCode && x.IsActive, cancellationToken);
    }

    public async Task<Role?> GetActiveRoleByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var roleId = await _context.UserRoles
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.IsActive)
            .OrderByDescending(x => x.AssignedAtUtc)
            .Select(x => x.RoleId)
            .FirstOrDefaultAsync(cancellationToken);

        if (roleId == Guid.Empty)
        {
            return null;
        }

        return await _context.Roles
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == roleId && x.IsActive, cancellationToken);
    }

    public async Task<IReadOnlyCollection<UserRole>> GetActiveUserRolesByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .Where(x => x.UserId == userId && x.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<RefreshToken>> GetActiveRefreshTokensByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
            .Where(x => x.UserId == userId && !x.IsRevoked)
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetUserByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public async Task<User?> GetUserByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == userId, cancellationToken);
    }

    public async Task<User?> GetUserByIdForUpdateAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .SingleOrDefaultAsync(x => x.Id == userId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<string>> GetActiveRoleCodesByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var roles = from userRole in _context.UserRoles
                    join role in _context.Roles on userRole.RoleId equals role.Id
                    where userRole.UserId == userId
                          && userRole.IsActive
                          && role.IsActive
                    select role.Code;

        return await roles.Distinct().ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<string>> GetActivePermissionCodesByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var permissions = from userRole in _context.UserRoles
                          join role in _context.Roles on userRole.RoleId equals role.Id
                          join rolePermission in _context.RolePermissions on role.Id equals rolePermission.RoleId
                          join permission in _context.Permissions on rolePermission.PermissionId equals permission.Id
                          where userRole.UserId == userId
                                && userRole.IsActive
                                && role.IsActive
                                && rolePermission.IsActive
                                && permission.IsActive
                          select permission.Code;

        return await permissions.Distinct().ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<AppointmentTherapistResponse>> ListActiveTherapistsAsync(
        CancellationToken cancellationToken = default)
    {
        var query = from user in _context.Users.AsNoTracking()
                    join userRole in _context.UserRoles.AsNoTracking() on user.Id equals userRole.UserId
                    join role in _context.Roles.AsNoTracking() on userRole.RoleId equals role.Id
                    where user.IsActive
                          && userRole.IsActive
                          && role.IsActive
                          && role.Code == "THERAPIST"
                    orderby user.FullName, user.Email
                    select new AppointmentTherapistResponse
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Email = user.Email
                    };

        return await query
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<RefreshToken?> GetRefreshTokenByHashAsync(
        string refreshTokenHash,
        CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
            .SingleOrDefaultAsync(x => x.TokenHash == refreshTokenHash, cancellationToken);
    }

    public async Task AddRefreshTokenAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default)
    {
        await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<User> BuildUsersSearchQuery(IQueryable<User> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var terms = search.Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var term in terms)
        {
            var pattern = $"%{term}%";
            query = query.Where(x =>
                EF.Functions.ILike(x.Email, pattern) ||
                EF.Functions.ILike(x.FullName, pattern));
        }

        return query;
    }
}
