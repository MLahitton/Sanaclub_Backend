namespace Sanaclub.Api.Contracts.Users;

public sealed class ResetUserPasswordRequest
{
    public string NewTemporaryPassword { get; init; } = string.Empty;
}

