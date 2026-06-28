using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Domain.Consents;
using Sanaclub.Domain.EvolutionSheets;
using Sanaclub.Domain.Patients;
using Sanaclub.Domain.TreatmentSheets;

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

    public GetPatientClinicalSummaryQueryHandler(
        IPatientRepository patientRepository,
        IConsentRepository consentRepository,
        ITreatmentSheetRepository treatmentSheetRepository,
        IEvolutionSheetRepository evolutionSheetRepository)
    {
        _patientRepository = patientRepository;
        _consentRepository = consentRepository;
        _treatmentSheetRepository = treatmentSheetRepository;
        _evolutionSheetRepository = evolutionSheetRepository;
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
            .Select(MapToTreatmentSheetResponse)
            .ToList()
            .AsReadOnly();

        var evolutionSheetResponses = evolutionSheets
            .Select(MapToEvolutionSheetResponse)
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
            Patient = MapToPatientResponse(patient),
            Counts = new PatientClinicalSummaryCountsResponse
            {
                TotalConsents = consents.Count,
                TotalTreatmentSheets = treatmentSheets.Count,
                TotalEvolutionSheets = evolutionSheets.Count,
                TotalApprovedTreatmentSheets = treatmentSheets.Count(x => x.ApprovedAtUtc.HasValue),
                TotalCompletedEvolutionSheets = evolutionSheets.Count(x => x.CompletedAtUtc.HasValue)
            },
            Consents = consents
                .Select(MapToConsentResponse)
                .ToList()
                .AsReadOnly(),
            TreatmentSheets = treatmentSheetResponses,
            EvolutionSheets = evolutionSheetResponses,
            LatestApprovedTreatmentSheet = latestApprovedTreatmentSheet,
            LatestCompletedEvolutionSheet = latestCompletedEvolutionSheet
        };
    }

    private static PatientClinicalSummaryPatientResponse MapToPatientResponse(Patient patient)
    {
        return new PatientClinicalSummaryPatientResponse
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

    private static PatientClinicalSummaryConsentResponse MapToConsentResponse(InformedConsent consent)
    {
        return new PatientClinicalSummaryConsentResponse
        {
            Id = consent.Id,
            DocumentTypeId = consent.DocumentTypeId,
            ConsentStatusId = consent.ConsentStatusId,
            Title = consent.Title,
            Description = consent.Description,
            SignedAtUtc = consent.SignedAtUtc,
            SignedByUserId = consent.SignedByUserId,
            PatientSignerName = consent.PatientSignerName,
            IsActive = consent.IsActive,
            CreatedAtUtc = consent.CreatedAtUtc
        };
    }

    private static PatientClinicalSummaryTreatmentSheetResponse MapToTreatmentSheetResponse(TreatmentSheet treatmentSheet)
    {
        return new PatientClinicalSummaryTreatmentSheetResponse
        {
            Id = treatmentSheet.Id,
            PatientId = treatmentSheet.PatientId,
            TreatmentStatusId = treatmentSheet.TreatmentStatusId,
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

    private static PatientClinicalSummaryEvolutionSheetResponse MapToEvolutionSheetResponse(EvolutionSheet evolutionSheet)
    {
        return new PatientClinicalSummaryEvolutionSheetResponse
        {
            Id = evolutionSheet.Id,
            PatientId = evolutionSheet.PatientId,
            TreatmentSheetId = evolutionSheet.TreatmentSheetId,
            EvolutionStatusId = evolutionSheet.EvolutionStatusId,
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
}
