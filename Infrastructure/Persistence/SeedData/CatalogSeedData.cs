namespace Sanaclub.Infrastructure.Persistence.SeedData;

public static class CatalogSeedData
{
    private static readonly SeedCatalogItem[] _identificationTypes =
    new[]
    {
        new SeedCatalogItem(
            new Guid("30000000-0000-0000-0000-000000000001"),
            "CC",
            "Cédula de ciudadanía",
            "Documento de identificación colombiano para ciudadano mayor de edad.",
            10),
        new SeedCatalogItem(
            new Guid("30000000-0000-0000-0000-000000000002"),
            "TI",
            "Tarjeta de identidad",
            "Documento de identificación colombiano para menores de edad.",
            20),
        new SeedCatalogItem(
            new Guid("30000000-0000-0000-0000-000000000003"),
            "CE",
            "Cédula de extranjería",
            "Documento de identificación para persona extranjera residente.",
            30),
        new SeedCatalogItem(
            new Guid("30000000-0000-0000-0000-000000000004"),
            "RC",
            "Registro civil",
            "Documento de identificación para menores registrados.",
            40),
        new SeedCatalogItem(
            new Guid("30000000-0000-0000-0000-000000000005"),
            "PASSPORT",
            "Pasaporte",
            "Documento internacional de identificación.",
            50),
        new SeedCatalogItem(
            new Guid("30000000-0000-0000-0000-000000000006"),
            "OTHER",
            "Otro",
            "Otro tipo de identificación.",
            60)
    };

    private static readonly SeedCatalogItem[] _genders =
    new[]
    {
        new SeedCatalogItem(
            new Guid("31000000-0000-0000-0000-000000000001"),
            "FEMALE",
            "Femenino",
            "Género femenino.",
            10),
        new SeedCatalogItem(
            new Guid("31000000-0000-0000-0000-000000000002"),
            "MALE",
            "Masculino",
            "Género masculino.",
            20),
        new SeedCatalogItem(
            new Guid("31000000-0000-0000-0000-000000000003"),
            "OTHER",
            "Otro",
            "Otro género.",
            30),
        new SeedCatalogItem(
            new Guid("31000000-0000-0000-0000-000000000004"),
            "NOT_SPECIFIED",
            "No especificado",
            "Género no especificado.",
            40)
    };

    private static readonly SeedCatalogItem[] _civilStatuses =
    new[]
    {
        new SeedCatalogItem(
            new Guid("32000000-0000-0000-0000-000000000001"),
            "SINGLE",
            "Soltero/a",
            "Estado civil soltero.",
            10),
        new SeedCatalogItem(
            new Guid("32000000-0000-0000-0000-000000000002"),
            "MARRIED",
            "Casado/a",
            "Estado civil casado.",
            20),
        new SeedCatalogItem(
            new Guid("32000000-0000-0000-0000-000000000003"),
            "FREE_UNION",
            "Unión libre",
            "Estado civil unión libre.",
            30),
        new SeedCatalogItem(
            new Guid("32000000-0000-0000-0000-000000000004"),
            "DIVORCED",
            "Divorciado/a",
            "Estado civil divorciado.",
            40),
        new SeedCatalogItem(
            new Guid("32000000-0000-0000-0000-000000000005"),
            "WIDOWED",
            "Viudo/a",
            "Estado civil viudo.",
            50),
        new SeedCatalogItem(
            new Guid("32000000-0000-0000-0000-000000000006"),
            "NOT_SPECIFIED",
            "No especificado",
            "Estado civil no especificado.",
            60)
    };

    private static readonly SeedCatalogItem[] _documentTypes =
    new[]
    {
        new SeedCatalogItem(
            new Guid("33000000-0000-0000-0000-000000000001"),
            "INFORMED_CONSENT",
            "Consentimiento informado",
            "Documento de consentimiento informado.",
            10),
        new SeedCatalogItem(
            new Guid("33000000-0000-0000-0000-000000000002"),
            "TREATMENT_SHEET",
            "Hoja de tratamiento",
            "Documento imprimible de hoja de tratamiento.",
            20),
        new SeedCatalogItem(
            new Guid("33000000-0000-0000-0000-000000000003"),
            "EVOLUTION_SHEET",
            "Hoja de evolución",
            "Documento imprimible de evolución de terapia.",
            30),
        new SeedCatalogItem(
            new Guid("33000000-0000-0000-0000-000000000004"),
            "PATIENT_SUMMARY",
            "Resumen del paciente",
            "Documento de resumen del historial del paciente.",
            40)
    };

    private static readonly SeedCatalogItem[] _patientStatuses =
    new[]
    {
        new SeedCatalogItem(
            new Guid("34000000-0000-0000-0000-000000000001"),
            "ACTIVE",
            "Activo",
            "Paciente activo en el sistema.",
            10),
        new SeedCatalogItem(
            new Guid("34000000-0000-0000-0000-000000000002"),
            "IN_TREATMENT",
            "En tratamiento",
            "Paciente con proceso terapéutico activo.",
            20),
        new SeedCatalogItem(
            new Guid("34000000-0000-0000-0000-000000000003"),
            "INACTIVE",
            "Inactivo",
            "Paciente inactivo.",
            30),
        new SeedCatalogItem(
            new Guid("34000000-0000-0000-0000-000000000004"),
            "ARCHIVED",
            "Archivado",
            "Paciente archivado.",
            40),
        new SeedCatalogItem(
            new Guid("34000000-0000-0000-0000-000000000005"),
            "CLOSED",
            "Cerrado",
            "Paciente con proceso cerrado.",
            50)
    };

    private static readonly SeedCatalogItem[] _consentStatuses =
    new[]
    {
        new SeedCatalogItem(
            new Guid("35000000-0000-0000-0000-000000000001"),
            "PENDING",
            "Pendiente",
            "Consentimiento pendiente de firma o decisión.",
            10),
        new SeedCatalogItem(
            new Guid("35000000-0000-0000-0000-000000000002"),
            "SIGNED",
            "Firmado",
            "Consentimiento firmado o aceptado.",
            20),
        new SeedCatalogItem(
            new Guid("35000000-0000-0000-0000-000000000003"),
            "REJECTED",
            "Rechazado",
            "Consentimiento rechazado por el paciente o representante.",
            30),
        new SeedCatalogItem(
            new Guid("35000000-0000-0000-0000-000000000004"),
            "REVOKED",
            "Revocado",
            "Consentimiento revocado.",
            40),
        new SeedCatalogItem(
            new Guid("35000000-0000-0000-0000-000000000005"),
            "REPLACED",
            "Reemplazado",
            "Consentimiento reemplazado por una versión posterior.",
            50)
    };

    private static readonly SeedCatalogItem[] _treatmentStatuses =
    new[]
    {
        new SeedCatalogItem(
            new Guid("36000000-0000-0000-0000-000000000001"),
            "DRAFT",
            "Borrador",
            "Hoja de tratamiento en borrador.",
            10),
        new SeedCatalogItem(
            new Guid("36000000-0000-0000-0000-000000000002"),
            "PENDING_MEDICAL_REVIEW",
            "Pendiente de revisión médica",
            "Hoja de tratamiento pendiente de revisión médica.",
            20),
        new SeedCatalogItem(
            new Guid("36000000-0000-0000-0000-000000000003"),
            "IN_MEDICAL_REVIEW",
            "En revisión médica",
            "Hoja de tratamiento en revisión médica.",
            30),
        new SeedCatalogItem(
            new Guid("36000000-0000-0000-0000-000000000004"),
            "APPROVED",
            "Aprobado",
            "Hoja de tratamiento aprobada por médico.",
            40),
        new SeedCatalogItem(
            new Guid("36000000-0000-0000-0000-000000000005"),
            "READY_TO_PRINT",
            "Lista para imprimir",
            "Hoja aprobada y lista para impresión.",
            50),
        new SeedCatalogItem(
            new Guid("36000000-0000-0000-0000-000000000006"),
            "PRINTED",
            "Impresa",
            "Hoja de tratamiento impresa.",
            60),
        new SeedCatalogItem(
            new Guid("36000000-0000-0000-0000-000000000007"),
            "IN_THERAPY",
            "En terapia",
            "Tratamiento en proceso de terapia.",
            70),
        new SeedCatalogItem(
            new Guid("36000000-0000-0000-0000-000000000008"),
            "CLOSED",
            "Cerrado",
            "Tratamiento cerrado.",
            80),
        new SeedCatalogItem(
            new Guid("36000000-0000-0000-0000-000000000009"),
            "CANCELLED",
            "Anulado",
            "Tratamiento anulado.",
            90)
    };

    private static readonly SeedCatalogItem[] _evolutionStatuses =
    new[]
    {
        new SeedCatalogItem(
            new Guid("37000000-0000-0000-0000-000000000001"),
            "DRAFT",
            "Borrador",
            "Evolución en borrador.",
            10),
        new SeedCatalogItem(
            new Guid("37000000-0000-0000-0000-000000000011"),
            "COMPLETED",
            "Completada",
            "Evolución completada.",
            15),
        new SeedCatalogItem(
            new Guid("37000000-0000-0000-0000-000000000002"),
            "PENDING_MEDICAL_REVIEW",
            "Pendiente de revisión médica",
            "Evolución pendiente de revisión médica.",
            20),
        new SeedCatalogItem(
            new Guid("37000000-0000-0000-0000-000000000003"),
            "IN_MEDICAL_REVIEW",
            "En revisión médica",
            "Evolución en revisión médica.",
            30),
        new SeedCatalogItem(
            new Guid("37000000-0000-0000-0000-000000000004"),
            "REVIEWED",
            "Revisada",
            "Evolución revisada por médico.",
            40),
        new SeedCatalogItem(
            new Guid("37000000-0000-0000-0000-000000000005"),
            "TREATMENT_KEPT",
            "Tratamiento mantenido",
            "Evolución revisada manteniendo tratamiento.",
            50),
        new SeedCatalogItem(
            new Guid("37000000-0000-0000-0000-000000000006"),
            "TREATMENT_MODIFIED",
            "Tratamiento modificado",
            "Evolución revisada con modificación de tratamiento.",
            60),
        new SeedCatalogItem(
            new Guid("37000000-0000-0000-0000-000000000007"),
            "READY_TO_PRINT",
            "Lista para imprimir",
            "Evolución lista para impresión.",
            70),
        new SeedCatalogItem(
            new Guid("37000000-0000-0000-0000-000000000008"),
            "PRINTED",
            "Impresa",
            "Evolución impresa.",
            80),
        new SeedCatalogItem(
            new Guid("37000000-0000-0000-0000-000000000009"),
            "CLOSED",
            "Cerrada",
            "Evolución cerrada.",
            90),
        new SeedCatalogItem(
            new Guid("37000000-0000-0000-0000-000000000010"),
            "CANCELLED",
            "Anulada",
            "Evolución anulada.",
            100)
    };

    public static IReadOnlyCollection<SeedCatalogItem> IdentificationTypes => _identificationTypes;
    public static IReadOnlyCollection<SeedCatalogItem> Genders => _genders;
    public static IReadOnlyCollection<SeedCatalogItem> CivilStatuses => _civilStatuses;
    public static IReadOnlyCollection<SeedCatalogItem> DocumentTypes => _documentTypes;
    public static IReadOnlyCollection<SeedCatalogItem> PatientStatuses => _patientStatuses;
    public static IReadOnlyCollection<SeedCatalogItem> ConsentStatuses => _consentStatuses;
    public static IReadOnlyCollection<SeedCatalogItem> TreatmentStatuses => _treatmentStatuses;
    public static IReadOnlyCollection<SeedCatalogItem> EvolutionStatuses => _evolutionStatuses;
}
