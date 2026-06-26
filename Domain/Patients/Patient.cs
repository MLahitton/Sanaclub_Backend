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
    public string? CityOrMunicipality { get; private set; }
    public string? Occupation { get; private set; }
    public string? EmergencyContactName { get; private set; }
    public string? EmergencyContactRelationship { get; private set; }
    public string? EmergencyContactPhone { get; private set; }
    public Guid PatientStatusId { get; private set; }
    public bool IsActive { get; private set; }

    public string FullName => $"{FirstName} {LastName}".Trim();

    private const int MaxIdentificationNumberLength = 50;
    private const int MaxFirstNameLength = 100;
    private const int MaxLastNameLength = 100;
    private const int MaxPhoneNumberLength = 50;
    private const int MaxEmailLength = 150;
    private const int MaxAddressLength = 250;
    private const int MaxCityOrMunicipalityLength = 150;
    private const int MaxOccupationLength = 150;
    private const int MaxEmergencyContactNameLength = 200;
    private const int MaxEmergencyContactRelationshipLength = 100;
    private const int MaxEmergencyContactPhoneLength = 50;

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
        string? cityOrMunicipality,
        string? occupation,
        string? emergencyContactName,
        string? emergencyContactRelationship,
        string? emergencyContactPhone,
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

        var trimmedCityOrMunicipality = string.IsNullOrWhiteSpace(cityOrMunicipality)
            ? null
            : cityOrMunicipality.Trim();
        if (trimmedCityOrMunicipality is not null && trimmedCityOrMunicipality.Length > MaxCityOrMunicipalityLength)
        {
            throw new DomainException("La ciudad o municipio no puede superar los 150 caracteres.");
        }

        var trimmedOccupation = string.IsNullOrWhiteSpace(occupation)
            ? null
            : occupation.Trim();
        if (trimmedOccupation is not null && trimmedOccupation.Length > MaxOccupationLength)
        {
            throw new DomainException("La ocupación no puede superar los 150 caracteres.");
        }

        var trimmedEmergencyContactName = string.IsNullOrWhiteSpace(emergencyContactName)
            ? null
            : emergencyContactName.Trim();
        if (trimmedEmergencyContactName is not null && trimmedEmergencyContactName.Length > MaxEmergencyContactNameLength)
        {
            throw new DomainException("El nombre de contacto de emergencia no puede superar los 200 caracteres.");
        }

        var trimmedEmergencyContactRelationship = string.IsNullOrWhiteSpace(emergencyContactRelationship)
            ? null
            : emergencyContactRelationship.Trim();
        if (trimmedEmergencyContactRelationship is not null && trimmedEmergencyContactRelationship.Length > MaxEmergencyContactRelationshipLength)
        {
            throw new DomainException("La relación de contacto de emergencia no puede superar los 100 caracteres.");
        }

        var trimmedEmergencyContactPhone = string.IsNullOrWhiteSpace(emergencyContactPhone)
            ? null
            : emergencyContactPhone.Trim();
        if (trimmedEmergencyContactPhone is not null && trimmedEmergencyContactPhone.Length > MaxEmergencyContactPhoneLength)
        {
            throw new DomainException("El teléfono de contacto de emergencia no puede superar los 50 caracteres.");
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
        CityOrMunicipality = trimmedCityOrMunicipality;
        Occupation = trimmedOccupation;
        EmergencyContactName = trimmedEmergencyContactName;
        EmergencyContactRelationship = trimmedEmergencyContactRelationship;
        EmergencyContactPhone = trimmedEmergencyContactPhone;
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
        string? cityOrMunicipality,
        string? occupation,
        string? emergencyContactName,
        string? emergencyContactRelationship,
        string? emergencyContactPhone,
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

        var trimmedCityOrMunicipality = string.IsNullOrWhiteSpace(cityOrMunicipality)
            ? null
            : cityOrMunicipality.Trim();
        if (trimmedCityOrMunicipality is not null && trimmedCityOrMunicipality.Length > MaxCityOrMunicipalityLength)
        {
            throw new DomainException("La ciudad o municipio no puede superar los 150 caracteres.");
        }

        var trimmedOccupation = string.IsNullOrWhiteSpace(occupation)
            ? null
            : occupation.Trim();
        if (trimmedOccupation is not null && trimmedOccupation.Length > MaxOccupationLength)
        {
            throw new DomainException("La ocupación no puede superar los 150 caracteres.");
        }

        var trimmedEmergencyContactName = string.IsNullOrWhiteSpace(emergencyContactName)
            ? null
            : emergencyContactName.Trim();
        if (trimmedEmergencyContactName is not null && trimmedEmergencyContactName.Length > MaxEmergencyContactNameLength)
        {
            throw new DomainException("El nombre de contacto de emergencia no puede superar los 200 caracteres.");
        }

        var trimmedEmergencyContactRelationship = string.IsNullOrWhiteSpace(emergencyContactRelationship)
            ? null
            : emergencyContactRelationship.Trim();
        if (trimmedEmergencyContactRelationship is not null && trimmedEmergencyContactRelationship.Length > MaxEmergencyContactRelationshipLength)
        {
            throw new DomainException("La relación de contacto de emergencia no puede superar los 100 caracteres.");
        }

        var trimmedEmergencyContactPhone = string.IsNullOrWhiteSpace(emergencyContactPhone)
            ? null
            : emergencyContactPhone.Trim();
        if (trimmedEmergencyContactPhone is not null && trimmedEmergencyContactPhone.Length > MaxEmergencyContactPhoneLength)
        {
            throw new DomainException("El teléfono de contacto de emergencia no puede superar los 50 caracteres.");
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
        CityOrMunicipality = trimmedCityOrMunicipality;
        Occupation = trimmedOccupation;
        EmergencyContactName = trimmedEmergencyContactName;
        EmergencyContactRelationship = trimmedEmergencyContactRelationship;
        EmergencyContactPhone = trimmedEmergencyContactPhone;
        PatientStatusId = patientStatusId;
        MarkAsUpdated(updatedByUserId);
    }

    public void Archive(Guid? archivedByUserId = null)
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        MarkAsUpdated(archivedByUserId);
    }
}
