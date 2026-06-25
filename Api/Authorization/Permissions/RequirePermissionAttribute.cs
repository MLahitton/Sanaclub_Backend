using Microsoft.AspNetCore.Authorization;

namespace Sanaclub.Api.Authorization.Permissions;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
        {
            throw new ArgumentException("Permission cannot be empty.", nameof(permission));
        }

        Policy = PermissionPolicyNames.For(permission);
    }
}
