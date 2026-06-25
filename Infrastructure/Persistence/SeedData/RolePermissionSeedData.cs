using System.Linq;

namespace Sanaclub.Infrastructure.Persistence.SeedData;

public static class RolePermissionSeedData
{
    private static readonly SeedRolePermission[] _rolePermissions =
    new[]
    {
        new SeedRolePermission(
            SeedId(1),
            GetRoleId("ADMIN"),
            GetPermissionId("auth.me"),
            "ADMIN",
            "auth.me"),
        new SeedRolePermission(
            SeedId(2),
            GetRoleId("ADMIN"),
            GetPermissionId("auth.logout"),
            "ADMIN",
            "auth.logout"),
        new SeedRolePermission(
            SeedId(3),
            GetRoleId("ADMIN"),
            GetPermissionId("auth.change_password"),
            "ADMIN",
            "auth.change_password"),
        new SeedRolePermission(
            SeedId(4),
            GetRoleId("ADMIN"),
            GetPermissionId("users.create"),
            "ADMIN",
            "users.create"),
        new SeedRolePermission(
            SeedId(5),
            GetRoleId("ADMIN"),
            GetPermissionId("users.read"),
            "ADMIN",
            "users.read"),
        new SeedRolePermission(
            SeedId(6),
            GetRoleId("ADMIN"),
            GetPermissionId("users.update"),
            "ADMIN",
            "users.update"),
        new SeedRolePermission(
            SeedId(7),
            GetRoleId("ADMIN"),
            GetPermissionId("users.disable"),
            "ADMIN",
            "users.disable"),
        new SeedRolePermission(
            SeedId(8),
            GetRoleId("ADMIN"),
            GetPermissionId("users.activate"),
            "ADMIN",
            "users.activate"),
        new SeedRolePermission(
            SeedId(9),
            GetRoleId("ADMIN"),
            GetPermissionId("roles.read"),
            "ADMIN",
            "roles.read"),
        new SeedRolePermission(
            SeedId(10),
            GetRoleId("ADMIN"),
            GetPermissionId("roles.assign"),
            "ADMIN",
            "roles.assign"),
        new SeedRolePermission(
            SeedId(11),
            GetRoleId("ADMIN"),
            GetPermissionId("roles.update_permissions"),
            "ADMIN",
            "roles.update_permissions"),
        new SeedRolePermission(
            SeedId(12),
            GetRoleId("ADMIN"),
            GetPermissionId("permissions.read"),
            "ADMIN",
            "permissions.read"),
        new SeedRolePermission(
            SeedId(13),
            GetRoleId("ADMIN"),
            GetPermissionId("patients.read"),
            "ADMIN",
            "patients.read"),
        new SeedRolePermission(
            SeedId(14),
            GetRoleId("ADMIN"),
            GetPermissionId("patients.archive"),
            "ADMIN",
            "patients.archive"),
        new SeedRolePermission(
            SeedId(104),
            GetRoleId("ADMIN"),
            GetPermissionId("patients.create"),
            "ADMIN",
            "patients.create"),
        new SeedRolePermission(
            SeedId(105),
            GetRoleId("ADMIN"),
            GetPermissionId("patients.update"),
            "ADMIN",
            "patients.update"),
        new SeedRolePermission(
            SeedId(15),
            GetRoleId("ADMIN"),
            GetPermissionId("consents.read_templates"),
            "ADMIN",
            "consents.read_templates"),
        new SeedRolePermission(
            SeedId(16),
            GetRoleId("ADMIN"),
            GetPermissionId("consents.manage_templates"),
            "ADMIN",
            "consents.manage_templates"),
        new SeedRolePermission(
            SeedId(17),
            GetRoleId("ADMIN"),
            GetPermissionId("documents.read"),
            "ADMIN",
            "documents.read"),
        new SeedRolePermission(
            SeedId(18),
            GetRoleId("ADMIN"),
            GetPermissionId("documents.cancel"),
            "ADMIN",
            "documents.cancel"),
        new SeedRolePermission(
            SeedId(19),
            GetRoleId("ADMIN"),
            GetPermissionId("documents.print_queue"),
            "ADMIN",
            "documents.print_queue"),
        new SeedRolePermission(
            SeedId(20),
            GetRoleId("ADMIN"),
            GetPermissionId("dashboard.admin"),
            "ADMIN",
            "dashboard.admin"),
        new SeedRolePermission(
            SeedId(21),
            GetRoleId("ADMIN"),
            GetPermissionId("catalogs.read"),
            "ADMIN",
            "catalogs.read"),
        new SeedRolePermission(
            SeedId(22),
            GetRoleId("ADMIN"),
            GetPermissionId("catalogs.manage"),
            "ADMIN",
            "catalogs.manage"),
        new SeedRolePermission(
            SeedId(23),
            GetRoleId("ADMIN"),
            GetPermissionId("audit.read"),
            "ADMIN",
            "audit.read"),
        new SeedRolePermission(
            SeedId(24),
            GetRoleId("ADMIN"),
            GetPermissionId("settings.read"),
            "ADMIN",
            "settings.read"),
        new SeedRolePermission(
            SeedId(25),
            GetRoleId("ADMIN"),
            GetPermissionId("settings.manage"),
            "ADMIN",
            "settings.manage"),

        new SeedRolePermission(
            SeedId(26),
            GetRoleId("DOCTOR"),
            GetPermissionId("auth.me"),
            "DOCTOR",
            "auth.me"),
        new SeedRolePermission(
            SeedId(27),
            GetRoleId("DOCTOR"),
            GetPermissionId("auth.logout"),
            "DOCTOR",
            "auth.logout"),
        new SeedRolePermission(
            SeedId(28),
            GetRoleId("DOCTOR"),
            GetPermissionId("auth.change_password"),
            "DOCTOR",
            "auth.change_password"),
        new SeedRolePermission(
            SeedId(29),
            GetRoleId("DOCTOR"),
            GetPermissionId("patients.create"),
            "DOCTOR",
            "patients.create"),
        new SeedRolePermission(
            SeedId(30),
            GetRoleId("DOCTOR"),
            GetPermissionId("patients.read"),
            "DOCTOR",
            "patients.read"),
        new SeedRolePermission(
            SeedId(31),
            GetRoleId("DOCTOR"),
            GetPermissionId("patients.read_sensitive"),
            "DOCTOR",
            "patients.read_sensitive"),
        new SeedRolePermission(
            SeedId(32),
            GetRoleId("DOCTOR"),
            GetPermissionId("patients.update"),
            "DOCTOR",
            "patients.update"),
        new SeedRolePermission(
            SeedId(33),
            GetRoleId("DOCTOR"),
            GetPermissionId("patients.read_history"),
            "DOCTOR",
            "patients.read_history"),
        new SeedRolePermission(
            SeedId(34),
            GetRoleId("DOCTOR"),
            GetPermissionId("consents.read_templates"),
            "DOCTOR",
            "consents.read_templates"),
        new SeedRolePermission(
            SeedId(35),
            GetRoleId("DOCTOR"),
            GetPermissionId("consents.create"),
            "DOCTOR",
            "consents.create"),
        new SeedRolePermission(
            SeedId(36),
            GetRoleId("DOCTOR"),
            GetPermissionId("consents.read"),
            "DOCTOR",
            "consents.read"),
        new SeedRolePermission(
            SeedId(37),
            GetRoleId("DOCTOR"),
            GetPermissionId("consents.sign"),
            "DOCTOR",
            "consents.sign"),
        new SeedRolePermission(
            SeedId(38),
            GetRoleId("DOCTOR"),
            GetPermissionId("consents.revoke"),
            "DOCTOR",
            "consents.revoke"),
        new SeedRolePermission(
            SeedId(39),
            GetRoleId("DOCTOR"),
            GetPermissionId("treatments.create"),
            "DOCTOR",
            "treatments.create"),
        new SeedRolePermission(
            SeedId(40),
            GetRoleId("DOCTOR"),
            GetPermissionId("treatments.read"),
            "DOCTOR",
            "treatments.read"),
        new SeedRolePermission(
            SeedId(41),
            GetRoleId("DOCTOR"),
            GetPermissionId("treatments.update_draft"),
            "DOCTOR",
            "treatments.update_draft"),
        new SeedRolePermission(
            SeedId(42),
            GetRoleId("DOCTOR"),
            GetPermissionId("treatments.submit_review"),
            "DOCTOR",
            "treatments.submit_review"),
        new SeedRolePermission(
            SeedId(43),
            GetRoleId("DOCTOR"),
            GetPermissionId("treatments.start_review"),
            "DOCTOR",
            "treatments.start_review"),
        new SeedRolePermission(
            SeedId(44),
            GetRoleId("DOCTOR"),
            GetPermissionId("treatments.review"),
            "DOCTOR",
            "treatments.review"),
        new SeedRolePermission(
            SeedId(45),
            GetRoleId("DOCTOR"),
            GetPermissionId("treatments.approve"),
            "DOCTOR",
            "treatments.approve"),
        new SeedRolePermission(
            SeedId(46),
            GetRoleId("DOCTOR"),
            GetPermissionId("treatments.modify"),
            "DOCTOR",
            "treatments.modify"),
        new SeedRolePermission(
            SeedId(47),
            GetRoleId("DOCTOR"),
            GetPermissionId("treatments.close"),
            "DOCTOR",
            "treatments.close"),
        new SeedRolePermission(
            SeedId(48),
            GetRoleId("DOCTOR"),
            GetPermissionId("treatments.cancel"),
            "DOCTOR",
            "treatments.cancel"),
        new SeedRolePermission(
            SeedId(49),
            GetRoleId("DOCTOR"),
            GetPermissionId("evolutions.create"),
            "DOCTOR",
            "evolutions.create"),
        new SeedRolePermission(
            SeedId(50),
            GetRoleId("DOCTOR"),
            GetPermissionId("evolutions.read"),
            "DOCTOR",
            "evolutions.read"),
        new SeedRolePermission(
            SeedId(51),
            GetRoleId("DOCTOR"),
            GetPermissionId("evolutions.update_draft"),
            "DOCTOR",
            "evolutions.update_draft"),
        new SeedRolePermission(
            SeedId(52),
            GetRoleId("DOCTOR"),
            GetPermissionId("evolutions.submit_review"),
            "DOCTOR",
            "evolutions.submit_review"),
        new SeedRolePermission(
            SeedId(53),
            GetRoleId("DOCTOR"),
            GetPermissionId("evolutions.start_review"),
            "DOCTOR",
            "evolutions.start_review"),
        new SeedRolePermission(
            SeedId(54),
            GetRoleId("DOCTOR"),
            GetPermissionId("evolutions.review"),
            "DOCTOR",
            "evolutions.review"),
        new SeedRolePermission(
            SeedId(55),
            GetRoleId("DOCTOR"),
            GetPermissionId("evolutions.keep_treatment"),
            "DOCTOR",
            "evolutions.keep_treatment"),
        new SeedRolePermission(
            SeedId(56),
            GetRoleId("DOCTOR"),
            GetPermissionId("evolutions.modify_treatment"),
            "DOCTOR",
            "evolutions.modify_treatment"),
        new SeedRolePermission(
            SeedId(57),
            GetRoleId("DOCTOR"),
            GetPermissionId("evolutions.close_process"),
            "DOCTOR",
            "evolutions.close_process"),
        new SeedRolePermission(
            SeedId(58),
            GetRoleId("DOCTOR"),
            GetPermissionId("evolutions.cancel"),
            "DOCTOR",
            "evolutions.cancel"),
        new SeedRolePermission(
            SeedId(59),
            GetRoleId("DOCTOR"),
            GetPermissionId("documents.generate"),
            "DOCTOR",
            "documents.generate"),
        new SeedRolePermission(
            SeedId(60),
            GetRoleId("DOCTOR"),
            GetPermissionId("documents.read"),
            "DOCTOR",
            "documents.read"),
        new SeedRolePermission(
            SeedId(61),
            GetRoleId("DOCTOR"),
            GetPermissionId("documents.download"),
            "DOCTOR",
            "documents.download"),
        new SeedRolePermission(
            SeedId(62),
            GetRoleId("DOCTOR"),
            GetPermissionId("documents.print"),
            "DOCTOR",
            "documents.print"),
        new SeedRolePermission(
            SeedId(63),
            GetRoleId("DOCTOR"),
            GetPermissionId("documents.cancel"),
            "DOCTOR",
            "documents.cancel"),
        new SeedRolePermission(
            SeedId(64),
            GetRoleId("DOCTOR"),
            GetPermissionId("dashboard.doctor"),
            "DOCTOR",
            "dashboard.doctor"),
        new SeedRolePermission(
            SeedId(65),
            GetRoleId("DOCTOR"),
            GetPermissionId("medical.review_queue"),
            "DOCTOR",
            "medical.review_queue"),
        new SeedRolePermission(
            SeedId(66),
            GetRoleId("DOCTOR"),
            GetPermissionId("catalogs.read"),
            "DOCTOR",
            "catalogs.read"),

        new SeedRolePermission(
            SeedId(67),
            GetRoleId("SECRETARY"),
            GetPermissionId("auth.me"),
            "SECRETARY",
            "auth.me"),
        new SeedRolePermission(
            SeedId(68),
            GetRoleId("SECRETARY"),
            GetPermissionId("auth.logout"),
            "SECRETARY",
            "auth.logout"),
        new SeedRolePermission(
            SeedId(69),
            GetRoleId("SECRETARY"),
            GetPermissionId("auth.change_password"),
            "SECRETARY",
            "auth.change_password"),
        new SeedRolePermission(
            SeedId(70),
            GetRoleId("SECRETARY"),
            GetPermissionId("patients.create"),
            "SECRETARY",
            "patients.create"),
        new SeedRolePermission(
            SeedId(71),
            GetRoleId("SECRETARY"),
            GetPermissionId("patients.read"),
            "SECRETARY",
            "patients.read"),
        new SeedRolePermission(
            SeedId(72),
            GetRoleId("SECRETARY"),
            GetPermissionId("patients.update"),
            "SECRETARY",
            "patients.update"),
        new SeedRolePermission(
            SeedId(73),
            GetRoleId("SECRETARY"),
            GetPermissionId("patients.read_history"),
            "SECRETARY",
            "patients.read_history"),
        new SeedRolePermission(
            SeedId(74),
            GetRoleId("SECRETARY"),
            GetPermissionId("consents.read_templates"),
            "SECRETARY",
            "consents.read_templates"),
        new SeedRolePermission(
            SeedId(75),
            GetRoleId("SECRETARY"),
            GetPermissionId("consents.create"),
            "SECRETARY",
            "consents.create"),
        new SeedRolePermission(
            SeedId(76),
            GetRoleId("SECRETARY"),
            GetPermissionId("consents.read"),
            "SECRETARY",
            "consents.read"),
        new SeedRolePermission(
            SeedId(77),
            GetRoleId("SECRETARY"),
            GetPermissionId("consents.sign"),
            "SECRETARY",
            "consents.sign"),
        new SeedRolePermission(
            SeedId(78),
            GetRoleId("SECRETARY"),
            GetPermissionId("treatments.create"),
            "SECRETARY",
            "treatments.create"),
        new SeedRolePermission(
            SeedId(79),
            GetRoleId("SECRETARY"),
            GetPermissionId("treatments.read"),
            "SECRETARY",
            "treatments.read"),
        new SeedRolePermission(
            SeedId(80),
            GetRoleId("SECRETARY"),
            GetPermissionId("treatments.update_draft"),
            "SECRETARY",
            "treatments.update_draft"),
        new SeedRolePermission(
            SeedId(81),
            GetRoleId("SECRETARY"),
            GetPermissionId("treatments.submit_review"),
            "SECRETARY",
            "treatments.submit_review"),
        new SeedRolePermission(
            SeedId(82),
            GetRoleId("SECRETARY"),
            GetPermissionId("evolutions.create"),
            "SECRETARY",
            "evolutions.create"),
        new SeedRolePermission(
            SeedId(83),
            GetRoleId("SECRETARY"),
            GetPermissionId("evolutions.read"),
            "SECRETARY",
            "evolutions.read"),
        new SeedRolePermission(
            SeedId(84),
            GetRoleId("SECRETARY"),
            GetPermissionId("evolutions.update_draft"),
            "SECRETARY",
            "evolutions.update_draft"),
        new SeedRolePermission(
            SeedId(85),
            GetRoleId("SECRETARY"),
            GetPermissionId("evolutions.submit_review"),
            "SECRETARY",
            "evolutions.submit_review"),
        new SeedRolePermission(
            SeedId(86),
            GetRoleId("SECRETARY"),
            GetPermissionId("documents.generate"),
            "SECRETARY",
            "documents.generate"),
        new SeedRolePermission(
            SeedId(87),
            GetRoleId("SECRETARY"),
            GetPermissionId("documents.read"),
            "SECRETARY",
            "documents.read"),
        new SeedRolePermission(
            SeedId(88),
            GetRoleId("SECRETARY"),
            GetPermissionId("documents.download"),
            "SECRETARY",
            "documents.download"),
        new SeedRolePermission(
            SeedId(89),
            GetRoleId("SECRETARY"),
            GetPermissionId("documents.print"),
            "SECRETARY",
            "documents.print"),
        new SeedRolePermission(
            SeedId(90),
            GetRoleId("SECRETARY"),
            GetPermissionId("documents.print_queue"),
            "SECRETARY",
            "documents.print_queue"),
        new SeedRolePermission(
            SeedId(91),
            GetRoleId("SECRETARY"),
            GetPermissionId("dashboard.secretary"),
            "SECRETARY",
            "dashboard.secretary"),
        new SeedRolePermission(
            SeedId(92),
            GetRoleId("SECRETARY"),
            GetPermissionId("medical.review_queue"),
            "SECRETARY",
            "medical.review_queue"),
        new SeedRolePermission(
            SeedId(93),
            GetRoleId("SECRETARY"),
            GetPermissionId("catalogs.read"),
            "SECRETARY",
            "catalogs.read"),

        new SeedRolePermission(
            SeedId(94),
            GetRoleId("THERAPIST"),
            GetPermissionId("auth.me"),
            "THERAPIST",
            "auth.me"),
        new SeedRolePermission(
            SeedId(95),
            GetRoleId("THERAPIST"),
            GetPermissionId("auth.logout"),
            "THERAPIST",
            "auth.logout"),
        new SeedRolePermission(
            SeedId(96),
            GetRoleId("THERAPIST"),
            GetPermissionId("auth.change_password"),
            "THERAPIST",
            "auth.change_password"),
        new SeedRolePermission(
            SeedId(106),
            GetRoleId("THERAPIST"),
            GetPermissionId("patients.read"),
            "THERAPIST",
            "patients.read"),
        new SeedRolePermission(
            SeedId(97),
            GetRoleId("THERAPIST"),
            GetPermissionId("catalogs.read"),
            "THERAPIST",
            "catalogs.read"),

        new SeedRolePermission(
            SeedId(98),
            GetRoleId("AUDITOR"),
            GetPermissionId("auth.me"),
            "AUDITOR",
            "auth.me"),
        new SeedRolePermission(
            SeedId(99),
            GetRoleId("AUDITOR"),
            GetPermissionId("auth.logout"),
            "AUDITOR",
            "auth.logout"),
        new SeedRolePermission(
            SeedId(100),
            GetRoleId("AUDITOR"),
            GetPermissionId("auth.change_password"),
            "AUDITOR",
            "auth.change_password"),
        new SeedRolePermission(
            SeedId(101),
            GetRoleId("AUDITOR"),
            GetPermissionId("audit.read"),
            "AUDITOR",
            "audit.read"),
        new SeedRolePermission(
            SeedId(102),
            GetRoleId("AUDITOR"),
            GetPermissionId("catalogs.read"),
            "AUDITOR",
            "catalogs.read"),
        new SeedRolePermission(
            SeedId(103),
            GetRoleId("AUDITOR"),
            GetPermissionId("settings.read"),
            "AUDITOR",
            "settings.read"),
        new SeedRolePermission(
            SeedId(107),
            GetRoleId("AUDITOR"),
            GetPermissionId("patients.read"),
            "AUDITOR",
            "patients.read")
    };

    public static IReadOnlyCollection<SeedRolePermission> RolePermissions => _rolePermissions;

    private static Guid SeedId(int number) => new Guid($"40000000-0000-0000-0000-{number:000000000000}");

    private static Guid GetRoleId(string roleCode) =>
        AuthSeedData.Roles.Single(role => role.Code == roleCode).Id;

    private static Guid GetPermissionId(string permissionCode) =>
        AuthSeedData.Permissions.Single(permission => permission.Code == permissionCode).Id;
}
