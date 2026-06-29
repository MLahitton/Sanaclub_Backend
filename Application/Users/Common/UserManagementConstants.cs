namespace Sanaclub.Application.Users.Common;

public static class UserManagementConstants
{
    public const string UsersReadPermission = "users.read";
    public const string UsersCreatePermission = "users.create";
    public const string UsersUpdatePermission = "users.update";
    public const string UsersChangeStatusPermission = "users.change_status";
    public const string UsersResetPasswordPermission = "users.reset_password";

    public const string AdminRoleCode = "ADMIN";
    public const string DoctorRoleCode = "DOCTOR";
    public const string SecretaryRoleCode = "SECRETARY";
    public const string TherapistRoleCode = "THERAPIST";
    public const string AuditorRoleCode = "AUDITOR";

    public const string UsersCreateRefreshReason = "User created";
    public const string UsersUpdateRoleReason = "User role updated";
    public const string UsersDeactivateReason = "User deactivated";
    public const string UsersResetPasswordReason = "Password reset";
    public const string UsersTokenRevokeReason = "Security refresh token revocation";
    public const string UsersDeactivateSelfConflictMessage = "An internal user cannot deactivate itself.";
    public const string LastActiveAdminConflictMessage = "The system would be left without an active ADMIN user.";

    public static readonly string[] InternalRoleCodes = new[]
    {
        AdminRoleCode,
        DoctorRoleCode,
        SecretaryRoleCode,
        TherapistRoleCode,
        AuditorRoleCode
    };

    public const int MaxNameLength = 100;
    public const int MaxFullNameLength = 200;
    public const int MaxEmailLength = 320;
    public const int MinPasswordLength = 8;
}
