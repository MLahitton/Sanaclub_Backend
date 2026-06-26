namespace Sanaclub.Api.Contracts.Consents;

public sealed class SignConsentRequest
{
    public string PatientSignerName { get; init; } = string.Empty;
}
