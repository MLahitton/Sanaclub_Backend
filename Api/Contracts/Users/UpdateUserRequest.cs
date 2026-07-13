namespace Sanaclub.Api.Contracts.Users;

public sealed class UpdateUserRequest
{
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string RoleCode { get; init; } = string.Empty;
}
