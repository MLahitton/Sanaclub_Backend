using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sanaclub.Infrastructure.Persistence;
using Sanaclub.Infrastructure.Persistence.SeedData;

namespace Sanaclub.Infrastructure.Persistence.Seeders;

public sealed class AuthSeeder : IDatabaseSeeder
{
    private readonly SanaclubDbContext _context;

    public AuthSeeder(SanaclubDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedRolesAsync(cancellationToken);
        await SeedPermissionsAsync(cancellationToken);
        await SeedRolePermissionsAsync(cancellationToken);
    }

    private async Task SeedRolesAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        foreach (var role in AuthSeedData.Roles)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $@"
                    INSERT INTO auth.roles (
                        id,
                        code,
                        name,
                        description,
                        is_active,
                        is_system_role,
                        created_at_utc,
                        created_by_user_id,
                        updated_at_utc,
                        updated_by_user_id)
                    VALUES (
                        {role.Id},
                        {role.Code},
                        {role.Name},
                        {role.Description},
                        true,
                        {role.IsSystemRole},
                        {now},
                        NULL,
                        NULL,
                        NULL)
                    ON CONFLICT (id) DO UPDATE SET
                        code = EXCLUDED.code,
                        name = EXCLUDED.name,
                        description = EXCLUDED.description,
                        is_active = true,
                        is_system_role = EXCLUDED.is_system_role,
                        updated_at_utc = {now},
                        updated_by_user_id = NULL;",
                cancellationToken);
        }
    }

    private async Task SeedPermissionsAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        foreach (var permission in AuthSeedData.Permissions)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $@"
                    INSERT INTO auth.permissions (
                        id,
                        code,
                        name,
                        description,
                        module,
                        is_active,
                        created_at_utc,
                        created_by_user_id,
                        updated_at_utc,
                        updated_by_user_id)
                    VALUES (
                        {permission.Id},
                        {permission.Code},
                        {permission.Name},
                        {permission.Description},
                        {permission.Module},
                        true,
                        {now},
                        NULL,
                        NULL,
                        NULL)
                    ON CONFLICT (id) DO UPDATE SET
                        code = EXCLUDED.code,
                        name = EXCLUDED.name,
                        description = EXCLUDED.description,
                        module = EXCLUDED.module,
                        is_active = true,
                        updated_at_utc = {now},
                        updated_by_user_id = NULL;",
                cancellationToken);
        }
    }

    private async Task SeedRolePermissionsAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        foreach (var rolePermission in RolePermissionSeedData.RolePermissions)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $@"
                    INSERT INTO auth.role_permissions (
                        id,
                        role_id,
                        permission_id,
                        assigned_at_utc,
                        assigned_by_user_id,
                        is_active,
                        revoked_at_utc,
                        revoked_by_user_id,
                        revocation_reason,
                        created_at_utc,
                        created_by_user_id,
                        updated_at_utc,
                        updated_by_user_id)
                    VALUES (
                        {rolePermission.Id},
                        {rolePermission.RoleId},
                        {rolePermission.PermissionId},
                        {now},
                        NULL,
                        true,
                        NULL,
                        NULL,
                        NULL,
                        {now},
                        NULL,
                        NULL,
                        NULL)
                    ON CONFLICT (role_id, permission_id) DO UPDATE SET
                        is_active = true,
                        revoked_at_utc = NULL,
                        revoked_by_user_id = NULL,
                        revocation_reason = NULL,
                        updated_at_utc = {now},
                        updated_by_user_id = NULL;",
                cancellationToken);
        }
    }
}
