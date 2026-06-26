using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.Consents.Common;
using Sanaclub.Domain.Consents;
using Sanaclub.Domain.Patients;

namespace Sanaclub.Application.Consents.ListByPatient;

public sealed class ListPatientConsentsQuery : IRequest<IReadOnlyCollection<ConsentResponse>>
{
    public Guid PatientId { get; init; }
}

public sealed class ListPatientConsentsQueryHandler : IRequestHandler<ListPatientConsentsQuery, IReadOnlyCollection<ConsentResponse>>
{
    private readonly IConsentRepository _consentRepository;
    private readonly IPatientRepository _patientRepository;

    public ListPatientConsentsQueryHandler(
        IConsentRepository consentRepository,
        IPatientRepository patientRepository)
    {
        _consentRepository = consentRepository;
        _patientRepository = patientRepository;
    }

    public async Task<IReadOnlyCollection<ConsentResponse>> Handle(ListPatientConsentsQuery request, CancellationToken cancellationToken)
    {
        if (request.PatientId == Guid.Empty)
        {
            throw new AppValidationException(
                "patientId",
                "El identificador del paciente es obligatorio.");
        }

        Patient? patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
        {
            throw new NotFoundException("El paciente no fue encontrado.");
        }

        var consents = await _consentRepository.ListByPatientIdAsync(request.PatientId, cancellationToken);
        return consents
            .Select(MapToResponse)
            .ToList()
            .AsReadOnly();
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

