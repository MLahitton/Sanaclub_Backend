using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.Patients.Common;
using Sanaclub.Domain.Patients;

namespace Sanaclub.Application.Patients.Archive;

public sealed class ArchivePatientCommand : IRequest<PatientResponse>
{
    public Guid PatientId { get; init; }
    public Guid ArchivedByUserId { get; init; }
}

public sealed class ArchivePatientCommandHandler : IRequestHandler<ArchivePatientCommand, PatientResponse>
{
    private readonly IPatientRepository _patientRepository;

    public ArchivePatientCommandHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<PatientResponse> Handle(ArchivePatientCommand request, CancellationToken cancellationToken)
    {
        if (request.PatientId == Guid.Empty)
        {
            throw new AppValidationException(
                "patientId",
                "El identificador del paciente es obligatorio.");
        }

        if (request.ArchivedByUserId == Guid.Empty)
        {
            throw new AppValidationException(
                "archivedByUserId",
                "El identificador del usuario archivador es obligatorio.");
        }

        var patient = await _patientRepository.GetByIdForUpdateAsync(request.PatientId, cancellationToken);

        if (patient is null)
        {
            throw new NotFoundException("El paciente no fue encontrado.");
        }

        patient.Archive(request.ArchivedByUserId);

        await _patientRepository.SaveChangesAsync(cancellationToken);

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
            PatientStatusId = patient.PatientStatusId,
            IsActive = patient.IsActive,
            CreatedAtUtc = patient.CreatedAtUtc
        };
    }
}
