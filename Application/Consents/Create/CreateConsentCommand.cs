using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.Consents.Common;
using Sanaclub.Domain.Consents;
using Sanaclub.Domain.Patients;

namespace Sanaclub.Application.Consents.Create;

public sealed class CreateConsentCommand : IRequest<ConsentResponse>
{
    public Guid PatientId { get; init; }
    public Guid DocumentTypeId { get; init; }
    public Guid ConsentStatusId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? PatientSignerName { get; init; }
    public string? Notes { get; init; }
    public Guid CreatedByUserId { get; init; }
}

public sealed class CreateConsentCommandHandler : IRequestHandler<CreateConsentCommand, ConsentResponse>
{
    private const int TitleMaxLength = 200;
    private const int DescriptionMaxLength = 1000;
    private const int PatientSignerNameMaxLength = 200;
    private const int NotesMaxLength = 1000;
    private const int MinTitleLength = 1;

    private readonly IConsentRepository _consentRepository;
    private readonly IPatientRepository _patientRepository;

    public CreateConsentCommandHandler(
        IConsentRepository consentRepository,
        IPatientRepository patientRepository)
    {
        _consentRepository = consentRepository;
        _patientRepository = patientRepository;
    }

    public async Task<ConsentResponse> Handle(CreateConsentCommand request, CancellationToken cancellationToken)
    {
        if (request.CreatedByUserId == Guid.Empty)
        {
            throw new AppValidationException(
                "createdByUserId",
                "El identificador del usuario creador es obligatorio.");
        }

        var trimmedTitle = string.IsNullOrWhiteSpace(request.Title)
            ? string.Empty
            : request.Title.Trim();
        var trimmedDescription = string.IsNullOrWhiteSpace(request.Description)
            ? null
            : request.Description.Trim();
        var trimmedPatientSignerName = string.IsNullOrWhiteSpace(request.PatientSignerName)
            ? null
            : request.PatientSignerName.Trim();
        var trimmedNotes = string.IsNullOrWhiteSpace(request.Notes)
            ? null
            : request.Notes.Trim();

        if (request.PatientId == Guid.Empty)
        {
            throw new AppValidationException(
                "patientId",
                "El identificador del paciente es obligatorio.");
        }

        if (request.DocumentTypeId == Guid.Empty)
        {
            throw new AppValidationException(
                "documentTypeId",
                "El tipo de documento es obligatorio.");
        }

        if (request.ConsentStatusId == Guid.Empty)
        {
            throw new AppValidationException(
                "consentStatusId",
                "El estado del consentimiento es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(trimmedTitle))
        {
            throw new AppValidationException(
                "title",
                "El título es obligatorio.");
        }

        if (trimmedTitle.Length < MinTitleLength)
        {
            throw new AppValidationException(
                "title",
                "El título no puede estar vacío.");
        }

        if (trimmedTitle.Length > TitleMaxLength)
        {
            throw new AppValidationException(
                "title",
                "El título no puede superar los 200 caracteres.");
        }

        if (trimmedDescription is not null && trimmedDescription.Length > DescriptionMaxLength)
        {
            throw new AppValidationException(
                "description",
                "La descripción no puede superar los 1000 caracteres.");
        }

        if (trimmedPatientSignerName is not null && trimmedPatientSignerName.Length > PatientSignerNameMaxLength)
        {
            throw new AppValidationException(
                "patientSignerName",
                "El nombre del firmante del paciente no puede superar los 200 caracteres.");
        }

        if (trimmedNotes is not null && trimmedNotes.Length > NotesMaxLength)
        {
            throw new AppValidationException(
                "notes",
                "Las notas no pueden superar los 1000 caracteres.");
        }

        Patient? patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);

        if (patient is null)
        {
            throw new NotFoundException("El paciente no fue encontrado.");
        }

        if (!patient.IsActive)
        {
            throw new ConflictException("No se puede crear un consentimiento para un paciente inactivo.");
        }

        var consent = new InformedConsent(
            request.PatientId,
            request.DocumentTypeId,
            request.ConsentStatusId,
            trimmedTitle,
            trimmedDescription,
            trimmedPatientSignerName,
            trimmedNotes);

        consent.MarkAsCreated(request.CreatedByUserId);

        await _consentRepository.AddAsync(consent, cancellationToken);
        await _consentRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(consent);
    }

    private static ConsentResponse MapToResponse(InformedConsent consent)
    {
        return new ConsentResponse
        {
            Id = consent.Id,
            PatientId = consent.PatientId,
            DocumentTypeId = consent.DocumentTypeId,
            ConsentStatusId = consent.ConsentStatusId,
            Title = consent.Title,
            Description = consent.Description,
            SignedAtUtc = consent.SignedAtUtc,
            SignedByUserId = consent.SignedByUserId,
            PatientSignerName = consent.PatientSignerName,
            Notes = consent.Notes,
            IsActive = consent.IsActive,
            CreatedAtUtc = consent.CreatedAtUtc
        };
    }
}

