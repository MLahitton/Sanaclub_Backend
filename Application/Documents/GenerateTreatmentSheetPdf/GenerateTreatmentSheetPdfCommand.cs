using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.Documents.Common;
using Sanaclub.Domain.Documents;
using Sanaclub.Domain.Patients;
using Sanaclub.Domain.TreatmentSheets;
using System.Globalization;

namespace Sanaclub.Application.Documents.GenerateTreatmentSheetPdf;

public sealed class GenerateTreatmentSheetPdfCommand : IRequest<GeneratedDocumentResponse>
{
    public Guid TreatmentSheetId { get; init; }
    public Guid GeneratedByUserId { get; init; }
}

public sealed class GenerateTreatmentSheetPdfCommandHandler :
    IRequestHandler<GenerateTreatmentSheetPdfCommand, GeneratedDocumentResponse>
{
    private readonly ITreatmentSheetRepository _treatmentSheetRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly ICatalogRepository _catalogRepository;
    private readonly ITreatmentSheetPdfGenerator _pdfGenerator;
    private readonly IDocumentStorage _documentStorage;
    private readonly IGeneratedDocumentRepository _generatedDocumentRepository;

    public GenerateTreatmentSheetPdfCommandHandler(
        ITreatmentSheetRepository treatmentSheetRepository,
        IPatientRepository patientRepository,
        ICatalogRepository catalogRepository,
        ITreatmentSheetPdfGenerator pdfGenerator,
        IDocumentStorage documentStorage,
        IGeneratedDocumentRepository generatedDocumentRepository)
    {
        _treatmentSheetRepository = treatmentSheetRepository;
        _patientRepository = patientRepository;
        _catalogRepository = catalogRepository;
        _pdfGenerator = pdfGenerator;
        _documentStorage = documentStorage;
        _generatedDocumentRepository = generatedDocumentRepository;
    }

    public async Task<GeneratedDocumentResponse> Handle(
        GenerateTreatmentSheetPdfCommand request,
        CancellationToken cancellationToken)
    {
        if (request.TreatmentSheetId == Guid.Empty)
        {
            throw new AppValidationException(
                "treatmentSheetId",
                "El identificador de la hoja de tratamiento es obligatorio.");
        }

        if (request.GeneratedByUserId == Guid.Empty)
        {
            throw new AppValidationException(
                "generatedByUserId",
                "El identificador del usuario generador es obligatorio.");
        }

        TreatmentSheet? treatmentSheet = await _treatmentSheetRepository.GetByIdAsync(
            request.TreatmentSheetId,
            cancellationToken);
        if (treatmentSheet is null)
        {
            throw new NotFoundException("La hoja de tratamiento no fue encontrada.");
        }

        Patient? patient = await _patientRepository.GetByIdAsync(
            treatmentSheet.PatientId,
            cancellationToken);
        if (patient is null)
        {
            throw new NotFoundException("El paciente no fue encontrado.");
        }

        var approvedStatusId = await _catalogRepository.GetTreatmentStatusIdByCodeAsync(
            "APPROVED",
            cancellationToken);
        if (!approvedStatusId.HasValue)
        {
            throw new ConflictException("El estado APPROVED de tratamiento no está configurado.");
        }

        if (treatmentSheet.TreatmentStatusId != approvedStatusId.Value)
        {
            throw new ConflictException("Solo se puede generar PDF de una hoja de tratamiento aprobada.");
        }

        byte[] pdf = await _pdfGenerator.GenerateAsync(patient, treatmentSheet, cancellationToken);

        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        var safeTreatmentNumber = string.IsNullOrWhiteSpace(treatmentSheet.TreatmentNumber)
            ? treatmentSheet.Id.ToString()
            : treatmentSheet.TreatmentNumber;
        var fileName = $"treatment-sheet-{treatmentSheet.Id}-{timestamp}.pdf";
        var storageObjectKey = $"patients/{patient.Id}/treatment-sheets/{treatmentSheet.Id}/{fileName}";

        var storedResult = await _documentStorage.SaveAsync(
            pdf,
            storageObjectKey,
            cancellationToken);

        var generatedDocument = new GeneratedDocument(
            patient.Id,
            "TREATMENT_SHEET",
            treatmentSheet.Id,
            $"Hoja de tratamiento - {safeTreatmentNumber}",
            fileName,
            "application/pdf",
            "LOCAL",
            storageObjectKey,
            storedResult.StorageBucket,
            storedResult.StoragePath,
            storedResult.FileSizeBytes,
            request.GeneratedByUserId,
            storedResult.StorageExternalId);

        generatedDocument.MarkAsCreated(request.GeneratedByUserId);

        await _generatedDocumentRepository.AddAsync(generatedDocument, cancellationToken);
        await _generatedDocumentRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(generatedDocument);
    }

    private static GeneratedDocumentResponse MapToResponse(GeneratedDocument document)
    {
        return new GeneratedDocumentResponse
        {
            Id = document.Id,
            PatientId = document.PatientId,
            DocumentKind = document.DocumentKind,
            SourceEntityId = document.SourceEntityId,
            Title = document.Title,
            FileName = document.FileName,
            ContentType = document.ContentType,
            StorageProvider = document.StorageProvider,
            StorageBucket = document.StorageBucket,
            StorageObjectKey = document.StorageObjectKey,
            StorageExternalId = document.StorageExternalId,
            StoragePath = document.StoragePath,
            FileSizeBytes = document.FileSizeBytes,
            Status = document.Status,
            GeneratedAtUtc = document.GeneratedAtUtc,
            GeneratedByUserId = document.GeneratedByUserId,
            IsActive = document.IsActive,
            CreatedAtUtc = document.CreatedAtUtc,
            UpdatedAtUtc = document.UpdatedAtUtc
        };
    }
}
