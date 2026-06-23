using Microsoft.EntityFrameworkCore;
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

    public async Task<User?> GetUserByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
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
}
