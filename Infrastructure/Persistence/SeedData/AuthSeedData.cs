namespace Sanaclub.Infrastructure.Persistence.SeedData;

public static class AuthSeedData
{
    private static readonly SeedRole[] _roles =
    new[]
    {
        new SeedRole(
            new Guid("10000000-0000-0000-0000-000000000001"),
            "ADMIN",
            "Administrador",
            "Acceso administrativo al sistema.",
            true),
        new SeedRole(
            new Guid("10000000-0000-0000-0000-000000000002"),
            "DOCTOR",
            "Médico",
            "Usuario médico encargado de revisar, indicar y aprobar tratamientos.",
            true),
        new SeedRole(
            new Guid("10000000-0000-0000-0000-000000000003"),
            "SECRETARY",
            "Secretaria",
            "Usuario encargado de registro, solicitudes, impresión y atención operativa.",
            true),
        new SeedRole(
            new Guid("10000000-0000-0000-0000-000000000004"),
            "THERAPIST",
            "Terapeuta",
            "Usuario terapeuta para acceso futuro limitado a terapias asignadas.",
            true),
        new SeedRole(
            new Guid("10000000-0000-0000-0000-000000000005"),
            "AUDITOR",
            "Auditor",
            "Usuario auditor para consulta futura de trazabilidad.",
            true)
    };

    private static readonly SeedPermission[] _permissions =
    new[]
    {
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000001"),
            "auth.me",
            "Consultar sesión actual",
            "auth",
            "Permite consultar la información básica de la sesión autenticada."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000002"),
            "auth.logout",
            "Cerrar sesión",
            "auth",
            "Permite cerrar la sesión actual."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000003"),
            "auth.change_password",
            "Cambiar contraseña",
            "auth",
            "Permite cambiar la contraseña propia."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000004"),
            "users.create",
            "Crear usuarios",
            "users",
            "Permite crear usuarios del sistema."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000005"),
            "users.read",
            "Consultar usuarios",
            "users",
            "Permite consultar usuarios del sistema."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000006"),
            "users.update",
            "Actualizar usuarios",
            "users",
            "Permite actualizar usuarios del sistema."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000007"),
            "users.disable",
            "Desactivar usuarios",
            "users",
            "Permite desactivar usuarios del sistema."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000008"),
            "users.activate",
            "Activar usuarios",
            "users",
            "Permite activar usuarios del sistema."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000061"),
            "users.change_status",
            "Cambiar estado de usuarios",
            "users",
            "Permite cambiar el estado activo/inactivo de usuarios del sistema."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000062"),
            "users.reset_password",
            "Reiniciar contraseÃ±a",
            "users",
            "Permite reiniciar la contraseÃ±a de usuarios del sistema."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000009"),
            "roles.read",
            "Consultar roles",
            "roles",
            "Permite consultar roles del sistema."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000010"),
            "roles.assign",
            "Asignar roles",
            "roles",
            "Permite asignar roles a usuarios."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000011"),
            "roles.update_permissions",
            "Actualizar permisos de roles",
            "roles",
            "Permite actualizar permisos asociados a roles."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000012"),
            "permissions.read",
            "Consultar permisos",
            "permissions",
            "Permite consultar permisos del sistema."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000013"),
            "patients.create",
            "Crear pacientes",
            "patients",
            "Permite registrar pacientes."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000014"),
            "patients.read",
            "Consultar pacientes",
            "patients",
            "Permite consultar pacientes."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000015"),
            "patients.read_sensitive",
            "Consultar información sensible de pacientes",
            "patients",
            "Permite consultar información sensible de pacientes cuando el rol lo autorice."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000016"),
            "patients.update",
            "Actualizar pacientes",
            "patients",
            "Permite actualizar datos de pacientes."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000017"),
            "patients.archive",
            "Archivar pacientes",
            "patients",
            "Permite archivar pacientes con trazabilidad."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000018"),
            "patients.read_history",
            "Consultar historial del paciente",
            "patients",
            "Permite consultar historial del paciente."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000019"),
            "consents.read_templates",
            "Consultar plantillas de consentimiento",
            "consents",
            "Permite consultar plantillas de consentimiento informado."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000020"),
            "consents.manage_templates",
            "Gestionar plantillas de consentimiento",
            "consents",
            "Permite crear o actualizar plantillas de consentimiento informado."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000021"),
            "consents.create",
            "Crear consentimientos",
            "consents",
            "Permite registrar consentimientos de pacientes."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000022"),
            "consents.read",
            "Consultar consentimientos",
            "consents",
            "Permite consultar consentimientos registrados."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000023"),
            "consents.sign",
            "Registrar firma de consentimiento",
            "consents",
            "Permite registrar la firma o aceptación del consentimiento."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000024"),
            "consents.revoke",
            "Revocar consentimiento",
            "consents",
            "Permite revocar un consentimiento con trazabilidad."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000025"),
            "treatments.create",
            "Crear hojas de tratamiento",
            "treatments",
            "Permite crear hojas de tratamiento."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000026"),
            "treatments.read",
            "Consultar hojas de tratamiento",
            "treatments",
            "Permite consultar hojas de tratamiento."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000027"),
            "treatments.update_draft",
            "Actualizar tratamiento en borrador",
            "treatments",
            "Permite actualizar hojas de tratamiento en estado borrador."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000028"),
            "treatments.submit_review",
            "Enviar tratamiento a revisión",
            "treatments",
            "Permite enviar una hoja de tratamiento a revisión médica."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000029"),
            "treatments.start_review",
            "Iniciar revisión de tratamiento",
            "treatments",
            "Permite iniciar revisión médica de una hoja de tratamiento."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000030"),
            "treatments.review",
            "Revisar tratamiento",
            "treatments",
            "Permite revisar clínicamente una hoja de tratamiento."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000031"),
            "treatments.approve",
            "Aprobar tratamiento",
            "treatments",
            "Permite aprobar indicaciones de tratamiento."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000032"),
            "treatments.modify",
            "Modificar tratamiento",
            "treatments",
            "Permite modificar indicaciones de tratamiento."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000033"),
            "treatments.close",
            "Cerrar tratamiento",
            "treatments",
            "Permite cerrar proceso de tratamiento."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000034"),
            "treatments.cancel",
            "Anular tratamiento",
            "treatments",
            "Permite anular tratamiento con motivo obligatorio."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000060"),
            "treatments.update_medical_indication",
            "Actualizar indicación médica",
            "treatments",
            "Permite actualizar la indicación médica de una hoja de tratamiento."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000035"),
            "evolutions.create",
            "Crear evoluciones",
            "evolutions",
            "Permite crear hojas de evolución."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000036"),
            "evolutions.read",
            "Consultar evoluciones",
            "evolutions",
            "Permite consultar hojas de evolución."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000037"),
            "evolutions.update_draft",
            "Actualizar evolución en borrador",
            "evolutions",
            "Permite actualizar evoluciones en estado borrador."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000038"),
            "evolutions.submit_review",
            "Enviar evolución a revisión",
            "evolutions",
            "Permite enviar una evolución a revisión médica."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000039"),
            "evolutions.start_review",
            "Iniciar revisión de evolución",
            "evolutions",
            "Permite iniciar revisión médica de una evolución."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000040"),
            "evolutions.review",
            "Revisar evolución",
            "evolutions",
            "Permite revisar clínicamente una evolución."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000041"),
            "evolutions.keep_treatment",
            "Mantener tratamiento",
            "evolutions",
            "Permite mantener indicaciones actuales tras revisar evolución."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000042"),
            "evolutions.modify_treatment",
            "Modificar tratamiento desde evolución",
            "evolutions",
            "Permite modificar indicaciones a partir de una evolución."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000043"),
            "evolutions.close_process",
            "Cerrar proceso desde evolución",
            "evolutions",
            "Permite cerrar proceso desde revisión de evolución."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000044"),
            "evolutions.cancel",
            "Anular evolución",
            "evolutions",
            "Permite anular evolución con motivo obligatorio."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000045"),
            "documents.generate",
            "Generar documentos",
            "documents",
            "Permite generar documentos PDF desde el backend."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000046"),
            "documents.read",
            "Consultar documentos",
            "documents",
            "Permite consultar documentos generados."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000047"),
            "documents.download",
            "Descargar documentos",
            "documents",
            "Permite descargar documentos mediante endpoint protegido."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000048"),
            "documents.print",
            "Registrar impresión de documentos",
            "documents",
            "Permite registrar impresión de documentos aprobados."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000049"),
            "documents.cancel",
            "Anular documentos",
            "documents",
            "Permite anular documentos generados con trazabilidad."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000050"),
            "documents.print_queue",
            "Consultar cola de impresión",
            "documents",
            "Permite consultar documentos pendientes o listos para impresión."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000051"),
            "dashboard.admin",
            "Ver dashboard administrativo",
            "dashboard",
            "Permite consultar el dashboard administrativo."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000052"),
            "dashboard.doctor",
            "Ver dashboard médico",
            "dashboard",
            "Permite consultar el dashboard médico."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000053"),
            "dashboard.secretary",
            "Ver dashboard de secretaria",
            "dashboard",
            "Permite consultar el dashboard operativo de secretaria."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000054"),
            "medical.review_queue",
            "Ver cola de revisión médica",
            "medical",
            "Permite consultar solicitudes pendientes de revisión médica."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000055"),
            "catalogs.read",
            "Consultar catálogos",
            "catalogs",
            "Permite consultar catálogos del sistema."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000056"),
            "catalogs.manage",
            "Gestionar catálogos",
            "catalogs",
            "Permite crear o actualizar catálogos del sistema."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000057"),
            "audit.read",
            "Consultar auditoría",
            "audit",
            "Permite consultar auditoría del sistema."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000058"),
            "settings.read",
            "Consultar configuración",
            "settings",
            "Permite consultar configuración del sistema."),
        new SeedPermission(
            new Guid("20000000-0000-0000-0000-000000000059"),
            "settings.manage",
            "Gestionar configuración",
            "settings",
            "Permite modificar configuración del sistema.")
    };

    public static IReadOnlyCollection<SeedRole> Roles => _roles;
    public static IReadOnlyCollection<SeedPermission> Permissions => _permissions;
}
