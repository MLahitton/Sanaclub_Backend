using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.Patients.Common;
using Sanaclub.Domain.Patients;

namespace Sanaclub.Application.Patients.GetById;

public sealed class GetPatientByIdQuery : IRequest<PatientResponse>
{
    public Guid PatientId { get; init; }
}

public sealed class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, PatientResponse>
{
    private readonly IPatientRepository _patientRepository;

    public GetPatientByIdQueryHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<PatientResponse> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.PatientId == Guid.Empty)
        {
            throw new AppValidationException(
                "patientId",
                "El identificador del paciente es obligatorio.");
        }

        var patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);

        if (patient is null)
        {
            throw new NotFoundException("El paciente no fue encontrado.");
        }

        return MapToResponse(patient);
    }

    private static PatientResponse MapToResponse(Patient patient)
    {
        return new PatientResponse
        {
            Id = patient.Id,
            IdentificationTypeId = patient.IdentificationTypeId,
            IdentificationNumber = patient.IdentificationNumber,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            FullName = patient.FullName,
            BirthDate = patient.BirthDate,
            GenderId = patient.GenderId,
            CivilStatusId = patient.CivilStatusId,
            PhoneNumber = patient.PhoneNumber,
            Email = patient.Email,
            Address = patient.Address,
            CityOrMunicipality = patient.CityOrMunicipality,
            Occupation = patient.Occupation,
            EmergencyContactName = patient.EmergencyContactName,
            EmergencyContactRelationship = patient.EmergencyContactRelationship,
            EmergencyContactPhone = patient.EmergencyContactPhone,
            PatientStatusId = patient.PatientStatusId,
            IsActive = patient.IsActive,
            CreatedAtUtc = patient.CreatedAtUtc
        };
    }
}
