using System.Globalization;
using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.Documents.Common;
using Sanaclub.Domain.Documents;
using Sanaclub.Domain.EvolutionSheets;
using Sanaclub.Domain.Patients;
using Sanaclub.Domain.TreatmentSheets;

namespace Sanaclub.Application.Documents.GenerateEvolutionSheetPdf;

public sealed class GenerateEvolutionSheetPdfCommand : IRequest<GeneratedDocumentResponse>
{
    public Guid EvolutionSheetId { get; init; }
    public Guid GeneratedByUserId { get; init; }
}

public sealed class GenerateEvolutionSheetPdfCommandHandler :
    IRequestHandler<GenerateEvolutionSheetPdfCommand, GeneratedDocumentResponse>
{
    private readonly IEvolutionSheetRepository _evolutionSheetRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly ITreatmentSheetRepository _treatmentSheetRepository;
    private readonly ICatalogRepository _catalogRepository;
    private readonly IEvolutionSheetPdfGenerator _pdfGenerator;
    private readonly IDocumentStorage _documentStorage;
    private readonly IGeneratedDocumentRepository _generatedDocumentRepository;

    public GenerateEvolutionSheetPdfCommandHandler(
        IEvolutionSheetRepository evolutionSheetRepository,
        IPatientRepository patientRepository,
        ITreatmentSheetRepository treatmentSheetRepository,
        ICatalogRepository catalogRepository,
        IEvolutionSheetPdfGenerator pdfGenerator,
        IDocumentStorage documentStorage,
        IGeneratedDocumentRepository generatedDocumentRepository)
    {
        _evolutionSheetRepository = evolutionSheetRepository;
        _patientRepository = patientRepository;
        _treatmentSheetRepository = treatmentSheetRepository;
        _catalogRepository = catalogRepository;
        _pdfGenerator = pdfGenerator;
        _documentStorage = documentStorage;
        _generatedDocumentRepository = generatedDocumentRepository;
    }

    public async Task<GeneratedDocumentResponse> Handle(
        GenerateEvolutionSheetPdfCommand request,
        CancellationToken cancellationToken)
    {
        if (request.EvolutionSheetId == Guid.Empty)
        {
            throw new AppValidationException(
                "evolutionSheetId",
                "El identificador de la hoja de evolucion es obligatorio.");
        }

        if (request.GeneratedByUserId == Guid.Empty)
        {
            throw new AppValidationException(
                "generatedByUserId",
                "El identificador del usuario generador es obligatorio.");
        }

        EvolutionSheet? evolutionSheet = await _evolutionSheetRepository.GetByIdAsync(
            request.EvolutionSheetId,
            cancellationToken);
        if (evolutionSheet is null)
        {
            throw new NotFoundException("La hoja de evolucion no fue encontrada.");
        }

        Patient? patient = await _patientRepository.GetByIdAsync(
            evolutionSheet.PatientId,
            cancellationToken);
        if (patient is null)
        {
            throw new NotFoundException("El paciente no fue encontrado.");
        }

        var completedStatusId = await _catalogRepository.GetEvolutionStatusIdByCodeAsync(
            "COMPLETED",
            cancellationToken);
        if (!completedStatusId.HasValue)
        {
            throw new ConflictException("El estado COMPLETED de evolución no está configurado.");
        }

        if (evolutionSheet.EvolutionStatusId != completedStatusId.Value)
        {
            throw new ConflictException("Solo se puede generar PDF de una hoja de evolución completada.");
        }

        TreatmentSheet? treatmentSheet = await _treatmentSheetRepository.GetByIdAsync(
            evolutionSheet.TreatmentSheetId,
            cancellationToken);
        if (treatmentSheet is null)
        {
            throw new NotFoundException("La hoja de tratamiento relacionada no fue encontrada.");
        }

        byte[] pdf = await _pdfGenerator.GenerateAsync(
            patient,
            evolutionSheet,
            treatmentSheet,
            cancellationToken);

        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        var safeEvolutionTitle = string.IsNullOrWhiteSpace(evolutionSheet.TherapyNumber)
            ? evolutionSheet.Id.ToString()
            : evolutionSheet.TherapyNumber;
        var fileName = $"evolution-sheet-{evolutionSheet.Id}-{timestamp}.pdf";
        var storageObjectKey = $"patients/{patient.Id}/evolution-sheets/{evolutionSheet.Id}/{fileName}";

        var storedResult = await _documentStorage.SaveAsync(
            pdf,
            storageObjectKey,
            cancellationToken);

        if (storedResult.FileSizeBytes <= 0)
        {
            throw new AppValidationException(
                "file",
                "El archivo PDF generado es nulo o esta vacio.");
        }

        var generatedDocument = new GeneratedDocument(
            patient.Id,
            "EVOLUTION_SHEET",
            evolutionSheet.Id,
            $"Hoja de evolución - {safeEvolutionTitle}",
            fileName,
            "application/pdf",
            storedResult.StorageProvider,
            storedResult.StorageObjectKey,
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
