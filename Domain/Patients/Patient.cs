using Sanaclub.Domain.Common;

namespace Sanaclub.Domain.Patients;

public sealed class Patient : AuditableEntity
{
    public Guid IdentificationTypeId { get; private set; }
    public string IdentificationNumber { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public DateOnly? BirthDate { get; private set; }
    public Guid? GenderId { get; private set; }
    public Guid? CivilStatusId { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Email { get; private set; }
    public string? Address { get; private set; }
    public Guid PatientStatusId { get; private set; }
    public bool IsActive { get; private set; }

    public string FullName => $"{FirstName} {LastName}".Trim();

    private const int MaxIdentificationNumberLength = 50;
    private const int MaxFirstNameLength = 100;
    private const int MaxLastNameLength = 100;
    private const int MaxPhoneNumberLength = 50;
    private const int MaxEmailLength = 150;
    private const int MaxAddressLength = 250;

    private Patient()
    {
    }

    public Patient(
        Guid identificationTypeId,
        string identificationNumber,
        string firstName,
        string lastName,
        DateOnly? birthDate,
        Guid? genderId,
        Guid? civilStatusId,
        string? phoneNumber,
        string? email,
        string? address,
        Guid patientStatusId)
    {
        if (identificationTypeId == Guid.Empty)
        {
            throw new DomainException("El tipo de identificación es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(identificationNumber))
        {
            throw new DomainException("El número de identificación es obligatorio.");
        }

        if (identificationNumber.Trim().Length > MaxIdentificationNumberLength)
        {
            throw new DomainException("El número de identificación no puede superar 50 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new DomainException("El nombre es obligatorio.");
        }

        if (firstName.Trim().Length > MaxFirstNameLength)
        {
            throw new DomainException("El nombre no puede superar 100 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new DomainException("El apellido es obligatorio.");
        }

        if (lastName.Trim().Length > MaxLastNameLength)
        {
            throw new DomainException("El apellido no puede superar 100 caracteres.");
        }

        if (birthDate.HasValue && birthDate.Value > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new DomainException("La fecha de nacimiento no puede ser futura.");
        }

        if (phoneNumber is not null && phoneNumber.Trim().Length > MaxPhoneNumberLength)
        {
            throw new DomainException("El número de teléfono no puede superar 50 caracteres.");
        }

        if (email is not null && email.Trim().Length > MaxEmailLength)
        {
            throw new DomainException("El correo no puede superar 150 caracteres.");
        }

        if (address is not null && address.Trim().Length > MaxAddressLength)
        {
            throw new DomainException("La dirección no puede superar 250 caracteres.");
        }

        if (patientStatusId == Guid.Empty)
        {
            throw new DomainException("El estado del paciente es obligatorio.");
        }

        IdentificationTypeId = identificationTypeId;
        IdentificationNumber = identificationNumber.Trim();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        BirthDate = birthDate;
        GenderId = genderId;
        CivilStatusId = civilStatusId;
        PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim();
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        Address = string.IsNullOrWhiteSpace(address) ? null : address.Trim();
        PatientStatusId = patientStatusId;
        IsActive = true;
    }

    public void UpdateBasicInfo(
        Guid identificationTypeId,
        string identificationNumber,
        string firstName,
        string lastName,
        DateOnly? birthDate,
        Guid? genderId,
        Guid? civilStatusId,
        string? phoneNumber,
        string? email,
        string? address,
        Guid patientStatusId,
        Guid? updatedByUserId = null)
    {
        if (identificationTypeId == Guid.Empty)
        {
            throw new DomainException("El tipo de identificación es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(identificationNumber))
        {
            throw new DomainException("El número de identificación es obligatorio.");
        }

        if (identificationNumber.Trim().Length > MaxIdentificationNumberLength)
        {
            throw new DomainException("El número de identificación no puede superar 50 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new DomainException("El nombre es obligatorio.");
        }

        if (firstName.Trim().Length > MaxFirstNameLength)
        {
            throw new DomainException("El nombre no puede superar 100 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new DomainException("El apellido es obligatorio.");
        }

        if (lastName.Trim().Length > MaxLastNameLength)
        {
            throw new DomainException("El apellido no puede superar 100 caracteres.");
        }

        if (birthDate.HasValue && birthDate.Value > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new DomainException("La fecha de nacimiento no puede ser futura.");
        }

        if (phoneNumber is not null && phoneNumber.Trim().Length > MaxPhoneNumberLength)
        {
            throw new DomainException("El número de teléfono no puede superar 50 caracteres.");
        }

        if (email is not null && email.Trim().Length > MaxEmailLength)
        {
            throw new DomainException("El correo no puede superar 150 caracteres.");
        }

        if (address is not null && address.Trim().Length > MaxAddressLength)
        {
            throw new DomainException("La dirección no puede superar 250 caracteres.");
        }

        if (patientStatusId == Guid.Empty)
        {
            throw new DomainException("El estado del paciente es obligatorio.");
        }

        IdentificationTypeId = identificationTypeId;
        IdentificationNumber = identificationNumber.Trim();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        BirthDate = birthDate;
        GenderId = genderId;
        CivilStatusId = civilStatusId;
        PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim();
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        Address = string.IsNullOrWhiteSpace(address) ? null : address.Trim();
        PatientStatusId = patientStatusId;
        MarkAsUpdated(updatedByUserId);
    }
}
