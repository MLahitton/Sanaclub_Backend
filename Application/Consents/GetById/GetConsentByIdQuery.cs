using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.Consents.Common;
using Sanaclub.Domain.Consents;

namespace Sanaclub.Application.Consents.GetById;

public sealed class GetConsentByIdQuery : IRequest<ConsentResponse>
{
    public Guid ConsentId { get; init; }
}

public sealed class GetConsentByIdQueryHandler : IRequestHandler<GetConsentByIdQuery, ConsentResponse>
{
    private readonly IConsentRepository _consentRepository;

    public GetConsentByIdQueryHandler(IConsentRepository consentRepository)
    {
        _consentRepository = consentRepository;
    }

    public async Task<ConsentResponse> Handle(GetConsentByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.ConsentId == Guid.Empty)
        {
            throw new AppValidationException(
                "consentId",
                "El identificador del consentimiento es obligatorio.");
        }

        var consent = await _consentRepository.GetByIdAsync(request.ConsentId, cancellationToken);
        if (consent is null)
        {
            throw new NotFoundException("El consentimiento no fue encontrado.");
        }

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

