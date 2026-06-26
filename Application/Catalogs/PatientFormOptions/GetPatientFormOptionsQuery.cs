using MediatR;
using Sanaclub.Application.Common.Abstractions;

namespace Sanaclub.Application.Catalogs.PatientFormOptions;

public sealed class GetPatientFormOptionsQuery : IRequest<PatientFormOptionsResponse>
{
}

public sealed class GetPatientFormOptionsQueryHandler
    : IRequestHandler<GetPatientFormOptionsQuery, PatientFormOptionsResponse>
{
    private readonly ICatalogRepository _catalogRepository;

    public GetPatientFormOptionsQueryHandler(ICatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public async Task<PatientFormOptionsResponse> Handle(
        GetPatientFormOptionsQuery request,
        CancellationToken cancellationToken)
    {
        var identificationTypes = await _catalogRepository.ListIdentificationTypesAsync(cancellationToken);
        var genders = await _catalogRepository.ListGendersAsync(cancellationToken);
        var civilStatuses = await _catalogRepository.ListCivilStatusesAsync(cancellationToken);
        var patientStatuses = await _catalogRepository.ListPatientStatusesAsync(cancellationToken);

        return new PatientFormOptionsResponse
        {
            IdentificationTypes = identificationTypes,
            Genders = genders,
            CivilStatuses = civilStatuses,
            PatientStatuses = patientStatuses
        };
    }
}
