using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.Consents.Common;
using Sanaclub.Domain.Consents;

namespace Sanaclub.Application.Consents.Sign;

public sealed class SignConsentCommand : IRequest<ConsentResponse>
{
    public Guid ConsentId { get; init; }
    public string PatientSignerName { get; init; } = string.Empty;
    public Guid SignedByUserId { get; init; }
}

public sealed class SignConsentCommandHandler : IRequestHandler<SignConsentCommand, ConsentResponse>
{
    private const int PatientSignerNameMaxLength = 200;

    private readonly IConsentRepository _consentRepository;
    private readonly ICatalogRepository _catalogRepository;

    public SignConsentCommandHandler(
        IConsentRepository consentRepository,
        ICatalogRepository catalogRepository)
    {
        _consentRepository = consentRepository;
        _catalogRepository = catalogRepository;
    }

    public async Task<ConsentResponse> Handle(SignConsentCommand request, CancellationToken cancellationToken)
    {
        if (request.ConsentId == Guid.Empty)
        {
            throw new AppValidationException(
                "consentId",
                "El identificador del consentimiento es obligatorio.");
        }

        if (request.SignedByUserId == Guid.Empty)
        {
            throw new AppValidationException(
                "signedByUserId",
                "El identificador del usuario autenticado es obligatorio.");
        }

        var trimmedPatientSignerName = string.IsNullOrWhiteSpace(request.PatientSignerName)
            ? string.Empty
            : request.PatientSignerName.Trim();

        if (string.IsNullOrWhiteSpace(trimmedPatientSignerName))
        {
            throw new AppValidationException(
                "patientSignerName",
                "El nombre del firmante del paciente es obligatorio.");
        }

        if (trimmedPatientSignerName.Length > PatientSignerNameMaxLength)
        {
            throw new AppValidationException(
                "patientSignerName",
                "El nombre del firmante del paciente no puede superar los 200 caracteres.");
        }

        var consent = await _consentRepository.GetByIdForUpdateAsync(request.ConsentId, cancellationToken);
        if (consent is null)
        {
            throw new NotFoundException("El consentimiento no fue encontrado.");
        }

        if (!consent.IsActive)
        {
            throw new ConflictException("El consentimiento está inactivo.");
        }

        if (consent.SignedAtUtc.HasValue)
        {
            return MapToResponse(consent);
        }

        var signedStatusId = await _catalogRepository.GetConsentStatusIdByCodeAsync("SIGNED", cancellationToken);
        if (!signedStatusId.HasValue)
        {
            throw new ConflictException("El estado SIGNED de consentimiento no está configurado.");
        }

        consent.MarkAsSigned(
            request.SignedByUserId,
            trimmedPatientSignerName,
            signedStatusId.Value);

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
