namespace Sanaclub.Api.Authorization.Permissions;

public static class PermissionPolicyNames
{
    public const string Prefix = "Permission:";

    public static string For(string permission)
    {
        return $"{Prefix}{permission}";
    }

    public static bool TryGetPermission(string policyName, out string permission)
    {
        permission = string.Empty;

        if (string.IsNullOrWhiteSpace(policyName))
        {
            return false;
        }

        if (!policyName.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        permission = policyName[Prefix.Length..];

        return !string.IsNullOrWhiteSpace(permission);
    }
}
