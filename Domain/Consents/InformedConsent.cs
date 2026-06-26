using Sanaclub.Domain.Common;

namespace Sanaclub.Domain.Consents;

public sealed class InformedConsent : AuditableEntity
{
    public Guid PatientId { get; private set; }
    public Guid DocumentTypeId { get; private set; }
    public Guid ConsentStatusId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTime? SignedAtUtc { get; private set; }
    public Guid? SignedByUserId { get; private set; }
    public string? PatientSignerName { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; }

    private const int TitleMaxLength = 200;
    private const int DescriptionMaxLength = 1000;
    private const int PatientSignerNameMaxLength = 200;
    private const int NotesMaxLength = 1000;

    private InformedConsent()
    {
    }

    public InformedConsent(
        Guid patientId,
        Guid documentTypeId,
        Guid consentStatusId,
        string title,
        string? description,
        string? patientSignerName,
        string? notes)
    {
        if (patientId == Guid.Empty)
        {
            throw new DomainException("El identificador del paciente es obligatorio.");
        }

        if (documentTypeId == Guid.Empty)
        {
            throw new DomainException("El tipo de documento es obligatorio.");
        }

        if (consentStatusId == Guid.Empty)
        {
            throw new DomainException("El estado del consentimiento es obligatorio.");
        }

        var trimmedTitle = string.IsNullOrWhiteSpace(title) ? string.Empty : title.Trim();
        if (string.IsNullOrWhiteSpace(trimmedTitle))
        {
            throw new DomainException("El título del consentimiento es obligatorio.");
        }

        if (trimmedTitle.Length > TitleMaxLength)
        {
            throw new DomainException("El título no puede superar los 200 caracteres.");
        }

        var trimmedDescription = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
        if (trimmedDescription is not null && trimmedDescription.Length > DescriptionMaxLength)
        {
            throw new DomainException("La descripción no puede superar los 1000 caracteres.");
        }

        var trimmedPatientSignerName = string.IsNullOrWhiteSpace(patientSignerName)
            ? null
            : patientSignerName.Trim();
        if (trimmedPatientSignerName is not null && trimmedPatientSignerName.Length > PatientSignerNameMaxLength)
        {
            throw new DomainException("El nombre del firmante del paciente no puede superar los 200 caracteres.");
        }

        var trimmedNotes = string.IsNullOrWhiteSpace(notes)
            ? null
            : notes.Trim();
        if (trimmedNotes is not null && trimmedNotes.Length > NotesMaxLength)
        {
            throw new DomainException("Las notas no pueden superar los 1000 caracteres.");
        }

        PatientId = patientId;
        DocumentTypeId = documentTypeId;
        ConsentStatusId = consentStatusId;
        Title = trimmedTitle;
        Description = trimmedDescription;
        PatientSignerName = trimmedPatientSignerName;
        Notes = trimmedNotes;
        IsActive = true;
    }

    public void MarkAsSigned(
        Guid? signedByUserId,
        string patientSignerName,
        DateTime? signedAtUtc = null)
    {
        var trimmedPatientSignerName = string.IsNullOrWhiteSpace(patientSignerName)
            ? null
            : patientSignerName.Trim();
        if (trimmedPatientSignerName is null)
        {
            throw new DomainException("El nombre del firmante del paciente es obligatorio.");
        }

        if (trimmedPatientSignerName.Length > PatientSignerNameMaxLength)
        {
            throw new DomainException("El nombre del firmante del paciente no puede superar los 200 caracteres.");
        }

        SignedByUserId = signedByUserId;
        SignedAtUtc = signedAtUtc ?? DateTime.UtcNow;
        PatientSignerName = trimmedPatientSignerName;

        MarkAsUpdated(signedByUserId);
    }

    public void Revoke(Guid? revokedByUserId = null)
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        MarkAsUpdated(revokedByUserId);
    }
}

