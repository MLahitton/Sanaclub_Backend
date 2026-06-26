using System;

namespace Sanaclub.Api.Contracts.Consents;

public sealed class CreateConsentRequest
{
    public Guid DocumentTypeId { get; init; }
    public Guid ConsentStatusId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? PatientSignerName { get; init; }
    public string? Notes { get; init; }
}

