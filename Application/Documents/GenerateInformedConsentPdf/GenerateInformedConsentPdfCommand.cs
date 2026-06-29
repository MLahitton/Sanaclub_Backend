using System.Globalization;
using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.Documents.Common;
using Sanaclub.Domain.Consents;
using Sanaclub.Domain.Documents;
using Sanaclub.Domain.Patients;

namespace Sanaclub.Application.Documents.GenerateInformedConsentPdf;

public sealed class GenerateInformedConsentPdfCommand : IRequest<GeneratedDocumentResponse>
{
    public Guid ConsentId { get; init; }
    public Guid GeneratedByUserId { get; init; }
}

public sealed class GenerateInformedConsentPdfCommandHandler :
    IRequestHandler<GenerateInformedConsentPdfCommand, GeneratedDocumentResponse>
{
    private readonly IConsentRepository _consentRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly ICatalogRepository _catalogRepository;
    private readonly IInformedConsentPdfGenerator _pdfGenerator;
    private readonly IDocumentStorage _documentStorage;
    private readonly IGeneratedDocumentRepository _generatedDocumentRepository;

    public GenerateInformedConsentPdfCommandHandler(
        IConsentRepository consentRepository,
        IPatientRepository patientRepository,
        ICatalogRepository catalogRepository,
        IInformedConsentPdfGenerator pdfGenerator,
        IDocumentStorage documentStorage,
        IGeneratedDocumentRepository generatedDocumentRepository)
    {
        _consentRepository = consentRepository;
        _patientRepository = patientRepository;
        _catalogRepository = catalogRepository;
        _pdfGenerator = pdfGenerator;
        _documentStorage = documentStorage;
        _generatedDocumentRepository = generatedDocumentRepository;
    }

    public async Task<GeneratedDocumentResponse> Handle(
        GenerateInformedConsentPdfCommand request,
        CancellationToken cancellationToken)
    {
        if (request.ConsentId == Guid.Empty)
        {
            throw new AppValidationException(
                "consentId",
                "El identificador del consentimiento es obligatorio.");
        }

        if (request.GeneratedByUserId == Guid.Empty)
        {
            throw new AppValidationException(
                "generatedByUserId",
                "El identificador del usuario generador es obligatorio.");
        }

        InformedConsent? consent = await _consentRepository.GetByIdAsync(request.ConsentId, cancellationToken);
        if (consent is null)
        {
            throw new NotFoundException("El consentimiento no fue encontrado.");
        }

        var patient = await _patientRepository.GetByIdAsync(consent.PatientId, cancellationToken);
        if (patient is null)
        {
            throw new NotFoundException("El paciente no fue encontrado.");
        }

        var signedStatusId = await _catalogRepository.GetConsentStatusIdByCodeAsync("SIGNED", cancellationToken);
        if (!signedStatusId.HasValue)
        {
            throw new ConflictException("El estado SIGNED de consentimiento no está configurado.");
        }

        if (consent.ConsentStatusId != signedStatusId.Value)
        {
            throw new ConflictException("Solo se puede generar PDF de un consentimiento firmado.");
        }

        var documentTypeName = await GetDocumentTypeNameAsync(consent.DocumentTypeId, cancellationToken);
        var identificationTypeName = await GetIdentificationTypeNameAsync(patient.IdentificationTypeId, cancellationToken);

        byte[] pdf = await _pdfGenerator.GenerateAsync(
            consent,
            patient,
            documentTypeName,
            identificationTypeName,
            cancellationToken);

        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        var safeConsentId = consent.Id;
        var fileName = $"informed-consent-{safeConsentId}-{timestamp}.pdf";
        var storageObjectKey = $"patients/{patient.Id}/consents/{consent.Id}/{fileName}";

        var storedResult = await _documentStorage.SaveAsync(
            pdf,
            storageObjectKey,
            cancellationToken);

        if (storedResult.FileSizeBytes <= 0)
        {
            throw new AppValidationException(
                "file",
                "El archivo PDF generado es nulo o está vacío.");
        }

        var safeTitle = string.IsNullOrWhiteSpace(documentTypeName)
            ? safeConsentId.ToString()
            : documentTypeName;

        var generatedDocument = new GeneratedDocument(
            patient.Id,
            "INFORMED_CONSENT",
            consent.Id,
            $"Consentimiento informado - {safeTitle}",
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

    private async Task<string?> GetDocumentTypeNameAsync(
        Guid documentTypeId,
        CancellationToken cancellationToken)
    {
        var documentTypes = await _catalogRepository.ListDocumentTypesAsync(cancellationToken);
        foreach (var documentType in documentTypes)
        {
            if (documentType.Id == documentTypeId)
            {
                return documentType.Name;
            }
        }

        return null;
    }

    private async Task<string?> GetIdentificationTypeNameAsync(
        Guid identificationTypeId,
        CancellationToken cancellationToken)
    {
        var identificationTypes = await _catalogRepository.ListIdentificationTypesAsync(cancellationToken);
        foreach (var identificationType in identificationTypes)
        {
            if (identificationType.Id == identificationTypeId)
            {
                return identificationType.Name;
            }
        }

        return null;
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
