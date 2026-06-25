using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Sanaclub.Api.Authorization.Permissions;

public sealed class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public PermissionAuthorizationPolicyProvider(
        IOptions<AuthorizationOptions> options)
        : base(options)
    {
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (PermissionPolicyNames.TryGetPermission(policyName, out var permission))
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new PermissionRequirement(permission))
                .Build();

            return policy;
        }

        return await base.GetPolicyAsync(policyName);
    }
}
