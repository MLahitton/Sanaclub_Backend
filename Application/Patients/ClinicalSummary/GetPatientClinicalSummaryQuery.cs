using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.Catalogs.Common;
using Sanaclub.Domain.Consents;
using Sanaclub.Domain.EvolutionSheets;
using Sanaclub.Domain.Patients;
using Sanaclub.Domain.TreatmentSheets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sanaclub.Application.Patients.ClinicalSummary;

public sealed class GetPatientClinicalSummaryQuery : IRequest<PatientClinicalSummaryResponse>
{
    public Guid PatientId { get; init; }
}

public sealed class GetPatientClinicalSummaryQueryHandler :
    IRequestHandler<GetPatientClinicalSummaryQuery, PatientClinicalSummaryResponse>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IConsentRepository _consentRepository;
    private readonly ITreatmentSheetRepository _treatmentSheetRepository;
    private readonly IEvolutionSheetRepository _evolutionSheetRepository;
    private readonly ICatalogRepository _catalogRepository;

    public GetPatientClinicalSummaryQueryHandler(
        IPatientRepository patientRepository,
        IConsentRepository consentRepository,
        ITreatmentSheetRepository treatmentSheetRepository,
        IEvolutionSheetRepository evolutionSheetRepository,
        ICatalogRepository catalogRepository)
    {
        _patientRepository = patientRepository;
        _consentRepository = consentRepository;
        _treatmentSheetRepository = treatmentSheetRepository;
        _evolutionSheetRepository = evolutionSheetRepository;
        _catalogRepository = catalogRepository;
    }

    public async Task<PatientClinicalSummaryResponse> Handle(
        GetPatientClinicalSummaryQuery request,
        CancellationToken cancellationToken)
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

        var identificationTypes = await _catalogRepository.ListIdentificationTypesAsync(cancellationToken);
        var genders = await _catalogRepository.ListGendersAsync(cancellationToken);
        var civilStatuses = await _catalogRepository.ListCivilStatusesAsync(cancellationToken);
        var patientStatuses = await _catalogRepository.ListPatientStatusesAsync(cancellationToken);
        var documentTypes = await _catalogRepository.ListDocumentTypesAsync(cancellationToken);
        var consentStatuses = await _catalogRepository.ListConsentStatusesAsync(cancellationToken);
        var treatmentStatuses = await _catalogRepository.ListTreatmentStatusesAsync(cancellationToken);
        var evolutionStatuses = await _catalogRepository.ListEvolutionStatusesAsync(cancellationToken);

        var identificationTypeById = identificationTypes.ToDictionary(x => x.Id);
        var genderById = genders.ToDictionary(x => x.Id);
        var civilStatusById = civilStatuses.ToDictionary(x => x.Id);
        var patientStatusById = patientStatuses.ToDictionary(x => x.Id);
        var documentTypeById = documentTypes.ToDictionary(x => x.Id);
        var consentStatusById = consentStatuses.ToDictionary(x => x.Id);
        var treatmentStatusById = treatmentStatuses.ToDictionary(x => x.Id);
        var evolutionStatusById = evolutionStatuses.ToDictionary(x => x.Id);

        var consents = await _consentRepository.ListByPatientIdAsync(
            request.PatientId,
            cancellationToken);

        var treatmentSheets = await _treatmentSheetRepository.ListByPatientIdAsync(
            request.PatientId,
            cancellationToken);

        var evolutionSheets = await _evolutionSheetRepository.ListByPatientIdAsync(
            request.PatientId,
            cancellationToken);

        var treatmentSheetResponses = treatmentSheets
            .Select(x => MapToTreatmentSheetResponse(x, treatmentStatusById))
            .ToList()
            .AsReadOnly();

        var evolutionSheetResponses = evolutionSheets
            .Select(x => MapToEvolutionSheetResponse(x, evolutionStatusById))
            .ToList()
            .AsReadOnly();

        var latestApprovedTreatmentSheet = treatmentSheetResponses
            .Where(x => x.ApprovedAtUtc.HasValue)
            .OrderByDescending(x => x.ApprovedAtUtc)
            .FirstOrDefault();

        var latestCompletedEvolutionSheet = evolutionSheetResponses
            .Where(x => x.CompletedAtUtc.HasValue)
            .OrderByDescending(x => x.CompletedAtUtc)
            .FirstOrDefault();

        return new PatientClinicalSummaryResponse
        {
            Patient = MapToPatientResponse(
                patient,
                identificationTypeById,
                genderById,
                civilStatusById,
                patientStatusById),
            Counts = new PatientClinicalSummaryCountsResponse
            {
                TotalConsents = consents.Count,
                TotalTreatmentSheets = treatmentSheets.Count,
                TotalEvolutionSheets = evolutionSheets.Count,
                TotalApprovedTreatmentSheets = treatmentSheets.Count(x => x.ApprovedAtUtc.HasValue),
                TotalCompletedEvolutionSheets = evolutionSheets.Count(x => x.CompletedAtUtc.HasValue)
            },
            Consents = consents
                .Select(x => MapToConsentResponse(x, documentTypeById, consentStatusById))
                .ToList()
                .AsReadOnly(),
            TreatmentSheets = treatmentSheetResponses,
            EvolutionSheets = evolutionSheetResponses,
            LatestApprovedTreatmentSheet = latestApprovedTreatmentSheet,
            LatestCompletedEvolutionSheet = latestCompletedEvolutionSheet
        };
    }

    private static PatientClinicalSummaryPatientResponse MapToPatientResponse(
        Patient patient,
        Dictionary<Guid, CatalogItemResponse> identificationTypeById,
        Dictionary<Guid, CatalogItemResponse> genderById,
        Dictionary<Guid, CatalogItemResponse> civilStatusById,
        Dictionary<Guid, CatalogItemResponse> patientStatusById)
    {
        var (identificationTypeCode, identificationTypeName) = GetCatalogCodeAndName(
            identificationTypeById,
            patient.IdentificationTypeId);

        var (genderCode, genderName) = GetCatalogCodeAndName(
            genderById,
            patient.GenderId);

        var (civilStatusCode, civilStatusName) = GetCatalogCodeAndName(
            civilStatusById,
            patient.CivilStatusId);

        var (patientStatusCode, patientStatusName) = GetCatalogCodeAndName(
            patientStatusById,
            patient.PatientStatusId);

        return new PatientClinicalSummaryPatientResponse
        {
            Id = patient.Id,
            IdentificationTypeId = patient.IdentificationTypeId,
            IdentificationTypeCode = identificationTypeCode,
            IdentificationTypeName = identificationTypeName,
            IdentificationNumber = patient.IdentificationNumber,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            FullName = patient.FullName,
            BirthDate = patient.BirthDate,
            GenderId = patient.GenderId,
            GenderCode = genderCode,
            GenderName = genderName,
            CivilStatusId = patient.CivilStatusId,
            CivilStatusCode = civilStatusCode,
            CivilStatusName = civilStatusName,
            PhoneNumber = patient.PhoneNumber,
            Email = patient.Email,
            Address = patient.Address,
            CityOrMunicipality = patient.CityOrMunicipality,
            Occupation = patient.Occupation,
            EmergencyContactName = patient.EmergencyContactName,
            EmergencyContactRelationship = patient.EmergencyContactRelationship,
            EmergencyContactPhone = patient.EmergencyContactPhone,
            PatientStatusId = patient.PatientStatusId,
            PatientStatusCode = patientStatusCode,
            PatientStatusName = patientStatusName,
            IsActive = patient.IsActive,
            CreatedAtUtc = patient.CreatedAtUtc
        };
    }

    private static PatientClinicalSummaryConsentResponse MapToConsentResponse(
        InformedConsent consent,
        Dictionary<Guid, CatalogItemResponse> documentTypeById,
        Dictionary<Guid, CatalogItemResponse> consentStatusById)
    {
        var (documentTypeCode, documentTypeName) = GetCatalogCodeAndName(
            documentTypeById,
            consent.DocumentTypeId);

        var (consentStatusCode, consentStatusName) = GetCatalogCodeAndName(
            consentStatusById,
            consent.ConsentStatusId);

        return new PatientClinicalSummaryConsentResponse
        {
            Id = consent.Id,
            DocumentTypeId = consent.DocumentTypeId,
            DocumentTypeCode = documentTypeCode,
            DocumentTypeName = documentTypeName,
            ConsentStatusId = consent.ConsentStatusId,
            ConsentStatusCode = consentStatusCode,
            ConsentStatusName = consentStatusName,
            Title = consent.Title,
            Description = consent.Description,
            SignedAtUtc = consent.SignedAtUtc,
            SignedByUserId = consent.SignedByUserId,
            PatientSignerName = consent.PatientSignerName,
            IsActive = consent.IsActive,
            CreatedAtUtc = consent.CreatedAtUtc
        };
    }

    private static PatientClinicalSummaryTreatmentSheetResponse MapToTreatmentSheetResponse(
        TreatmentSheet treatmentSheet,
        Dictionary<Guid, CatalogItemResponse> treatmentStatusById)
    {
        var (treatmentStatusCode, treatmentStatusName) = GetCatalogCodeAndName(
            treatmentStatusById,
            treatmentSheet.TreatmentStatusId);

        return new PatientClinicalSummaryTreatmentSheetResponse
        {
            Id = treatmentSheet.Id,
            PatientId = treatmentSheet.PatientId,
            TreatmentStatusId = treatmentSheet.TreatmentStatusId,
            TreatmentStatusCode = treatmentStatusCode,
            TreatmentStatusName = treatmentStatusName,
            TreatmentNumber = treatmentSheet.TreatmentNumber,
            ConsultationDate = treatmentSheet.ConsultationDate,
            EpsTreatingDoctorDiagnosis = treatmentSheet.EpsTreatingDoctorDiagnosis,
            ReferredClinicalHistory = treatmentSheet.ReferredClinicalHistory,
            IndicationDate = treatmentSheet.IndicationDate,
            EntryTime = treatmentSheet.EntryTime,
            ExitTime = treatmentSheet.ExitTime,
            AssignedStaffName = treatmentSheet.AssignedStaffName,
            TherapyName = treatmentSheet.TherapyName,
            NervousSystemIndications = treatmentSheet.NervousSystemIndications,
            DecompressSpine = treatmentSheet.DecompressSpine,
            DecompressNeck = treatmentSheet.DecompressNeck,
            DecompressBack = treatmentSheet.DecompressBack,
            EndocrineNerves = treatmentSheet.EndocrineNerves,
            EndocrineDefenses = treatmentSheet.EndocrineDefenses,
            EndocrineHormones = treatmentSheet.EndocrineHormones,
            CardiovascularReflexologyWith = treatmentSheet.CardiovascularReflexologyWith,
            DigestiveColonReflexologyWith = treatmentSheet.DigestiveColonReflexologyWith,
            RespiratoryReflexologyWith = treatmentSheet.RespiratoryReflexologyWith,
            UrinaryReflexologyWithAcidFruits = treatmentSheet.UrinaryReflexologyWithAcidFruits,
            OtherIndications = treatmentSheet.OtherIndications,
            Observations = treatmentSheet.Observations,
            ApprovedAtUtc = treatmentSheet.ApprovedAtUtc,
            ApprovedByUserId = treatmentSheet.ApprovedByUserId,
            IsActive = treatmentSheet.IsActive,
            CreatedAtUtc = treatmentSheet.CreatedAtUtc,
            UpdatedAtUtc = treatmentSheet.UpdatedAtUtc
        };
    }

    private static PatientClinicalSummaryEvolutionSheetResponse MapToEvolutionSheetResponse(
        EvolutionSheet evolutionSheet,
        Dictionary<Guid, CatalogItemResponse> evolutionStatusById)
    {
        var (evolutionStatusCode, evolutionStatusName) = GetCatalogCodeAndName(
            evolutionStatusById,
            evolutionSheet.EvolutionStatusId);

        return new PatientClinicalSummaryEvolutionSheetResponse
        {
            Id = evolutionSheet.Id,
            PatientId = evolutionSheet.PatientId,
            TreatmentSheetId = evolutionSheet.TreatmentSheetId,
            EvolutionStatusId = evolutionSheet.EvolutionStatusId,
            EvolutionStatusCode = evolutionStatusCode,
            EvolutionStatusName = evolutionStatusName,
            TherapyNumber = evolutionSheet.TherapyNumber,
            EvolutionDate = evolutionSheet.EvolutionDate,
            EntryTime = evolutionSheet.EntryTime,
            ExitTime = evolutionSheet.ExitTime,
            AssignedStaffName = evolutionSheet.AssignedStaffName,
            TherapyName = evolutionSheet.TherapyName,
            EvolutionNotes = evolutionSheet.EvolutionNotes,
            NewIndications = evolutionSheet.NewIndications,
            CompletedAtUtc = evolutionSheet.CompletedAtUtc,
            CompletedByUserId = evolutionSheet.CompletedByUserId,
            IsActive = evolutionSheet.IsActive,
            CreatedAtUtc = evolutionSheet.CreatedAtUtc,
            UpdatedAtUtc = evolutionSheet.UpdatedAtUtc
        };
    }

    private static (string? code, string? name) GetCatalogCodeAndName(
        Dictionary<Guid, CatalogItemResponse> catalogById,
        Guid? id)
    {
        if (!id.HasValue)
        {
            return (null, null);
        }

        return catalogById.TryGetValue(id.Value, out var item)
            ? (item.Code, item.Name)
            : (null, null);
    }
}
