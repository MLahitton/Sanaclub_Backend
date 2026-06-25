using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.Patients.Common;
using Sanaclub.Domain.Patients;

namespace Sanaclub.Application.Patients.Update;

public sealed class UpdatePatientCommand : IRequest<PatientResponse>
{
    public Guid PatientId { get; init; }
    public Guid IdentificationTypeId { get; init; }
    public string IdentificationNumber { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public DateOnly? BirthDate { get; init; }
    public Guid? GenderId { get; init; }
    public Guid? CivilStatusId { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public string? Address { get; init; }
    public Guid PatientStatusId { get; init; }
    public Guid UpdatedByUserId { get; init; }
}

public sealed class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, PatientResponse>
{
    private const int IdentificationNumberMaxLength = 50;
    private const int NameMaxLength = 100;
    private const int PhoneNumberMaxLength = 50;
    private const int EmailMaxLength = 150;
    private const int AddressMaxLength = 250;
    private const int MinNameLength = 1;

    private readonly IPatientRepository _patientRepository;

    public UpdatePatientCommandHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<PatientResponse> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        if (request.PatientId == Guid.Empty)
        {
            throw new AppValidationException(
                "patientId",
                "El identificador del paciente es obligatorio.");
        }

        if (request.UpdatedByUserId == Guid.Empty)
        {
            throw new AppValidationException(
                "updatedByUserId",
                "El identificador del usuario actualizador es obligatorio.");
        }

        var trimmedIdentificationNumber = string.IsNullOrWhiteSpace(request.IdentificationNumber)
            ? string.Empty
            : request.IdentificationNumber.Trim();
        var trimmedFirstName = string.IsNullOrWhiteSpace(request.FirstName)
            ? string.Empty
            : request.FirstName.Trim();
        var trimmedLastName = string.IsNullOrWhiteSpace(request.LastName)
            ? string.Empty
            : request.LastName.Trim();
        var trimmedPhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber)
            ? null
            : request.PhoneNumber.Trim();
        var trimmedEmail = string.IsNullOrWhiteSpace(request.Email)
            ? null
            : request.Email.Trim();
        var trimmedAddress = string.IsNullOrWhiteSpace(request.Address)
            ? null
            : request.Address.Trim();

        if (request.IdentificationTypeId == Guid.Empty)
        {
            throw new AppValidationException(
                "identificationTypeId",
                "El tipo de identificación es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(trimmedIdentificationNumber))
        {
            throw new AppValidationException(
                "identificationNumber",
                "El número de identificación es obligatorio.");
        }

        if (trimmedIdentificationNumber.Length > IdentificationNumberMaxLength)
        {
            throw new AppValidationException(
                "identificationNumber",
                "El número de identificación no puede superar los 50 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(trimmedFirstName))
        {
            throw new AppValidationException(
                "firstName",
                "El nombre es obligatorio.");
        }

        if (trimmedFirstName.Length > NameMaxLength)
        {
            throw new AppValidationException(
                "firstName",
                "El nombre no puede superar los 100 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(trimmedLastName))
        {
            throw new AppValidationException(
                "lastName",
                "El apellido es obligatorio.");
        }

        if (trimmedLastName.Length > NameMaxLength)
        {
            throw new AppValidationException(
                "lastName",
                "El apellido no puede superar los 100 caracteres.");
        }

        if (request.BirthDate.HasValue && request.BirthDate.Value > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new AppValidationException(
                "birthDate",
                "La fecha de nacimiento no puede ser futura.");
        }

        if (trimmedPhoneNumber is not null && trimmedPhoneNumber.Length > PhoneNumberMaxLength)
        {
            throw new AppValidationException(
                "phoneNumber",
                "El número de teléfono no puede superar los 50 caracteres.");
        }

        if (trimmedEmail is not null && trimmedEmail.Length > EmailMaxLength)
        {
            throw new AppValidationException(
                "email",
                "El correo no puede superar los 150 caracteres.");
        }

        if (trimmedAddress is not null && trimmedAddress.Length > AddressMaxLength)
        {
            throw new AppValidationException(
                "address",
                "La dirección no puede superar los 250 caracteres.");
        }

        if (request.PatientStatusId == Guid.Empty)
        {
            throw new AppValidationException(
                "patientStatusId",
                "El estado del paciente es obligatorio.");
        }

        if (trimmedFirstName.Length < MinNameLength)
        {
            throw new AppValidationException(
                "firstName",
                "El nombre no puede estar vacío.");
        }

        if (trimmedLastName.Length < MinNameLength)
        {
            throw new AppValidationException(
                "lastName",
                "El apellido no puede estar vacío.");
        }

        var patient = await _patientRepository.GetByIdForUpdateAsync(
            request.PatientId,
            cancellationToken);

        if (patient is null)
        {
            throw new NotFoundException("El paciente no fue encontrado.");
        }

        var exists = await _patientRepository.ExistsByIdentificationExcludingPatientAsync(
            request.PatientId,
            request.IdentificationTypeId,
            trimmedIdentificationNumber,
            cancellationToken);

        if (exists)
        {
            throw new ConflictException("Ya existe un paciente con ese tipo y número de identificación.");
        }

        patient.UpdateBasicInfo(
            request.IdentificationTypeId,
            trimmedIdentificationNumber,
            trimmedFirstName,
            trimmedLastName,
            request.BirthDate,
            request.GenderId,
            request.CivilStatusId,
            trimmedPhoneNumber,
            trimmedEmail,
            trimmedAddress,
            request.PatientStatusId,
            request.UpdatedByUserId);

        await _patientRepository.SaveChangesAsync(cancellationToken);

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
            PatientStatusId = patient.PatientStatusId,
            IsActive = patient.IsActive,
            CreatedAtUtc = patient.CreatedAtUtc
        };
    }
}
