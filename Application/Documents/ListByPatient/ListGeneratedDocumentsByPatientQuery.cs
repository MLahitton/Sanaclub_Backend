using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.Documents.Common;
using System.Linq;
using Sanaclub.Domain.Documents;
using Sanaclub.Domain.Patients;

namespace Sanaclub.Application.Documents.ListByPatient;

public sealed class ListGeneratedDocumentsByPatientQuery : IRequest<IReadOnlyList<GeneratedDocumentResponse>>
{
    public Guid PatientId { get; init; }
}

public sealed class ListGeneratedDocumentsByPatientQueryHandler :
    IRequestHandler<ListGeneratedDocumentsByPatientQuery, IReadOnlyList<GeneratedDocumentResponse>>
{
    private readonly IGeneratedDocumentRepository _generatedDocumentRepository;
    private readonly IPatientRepository _patientRepository;

    public ListGeneratedDocumentsByPatientQueryHandler(
        IGeneratedDocumentRepository generatedDocumentRepository,
        IPatientRepository patientRepository)
    {
        _generatedDocumentRepository = generatedDocumentRepository;
        _patientRepository = patientRepository;
    }

    public async Task<IReadOnlyList<GeneratedDocumentResponse>> Handle(
        ListGeneratedDocumentsByPatientQuery request,
        CancellationToken cancellationToken)
    {
        if (request.PatientId == Guid.Empty)
        {
            throw new AppValidationException(
                "patientId",
                "El identificador del paciente es obligatorio.");
        }

        Patient? patient = await _patientRepository.GetByIdAsync(
            request.PatientId,
            cancellationToken);

        if (patient is null)
        {
            throw new NotFoundException("El paciente no fue encontrado.");
        }

        var documents = await _generatedDocumentRepository.ListByPatientIdAsync(
            patient.Id,
            cancellationToken);

        return documents
            .Select(MapToResponse)
            .ToList()
            .AsReadOnly();
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
