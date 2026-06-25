using Microsoft.AspNetCore.Authorization;

namespace Sanaclub.Api.Authorization.Permissions;

public sealed class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
        {
            throw new ArgumentException("Permission cannot be empty.", nameof(permission));
        }

        Permission = permission;
    }

    public string Permission { get; }
}
