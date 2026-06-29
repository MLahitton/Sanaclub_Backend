namespace Sanaclub.Api.Contracts.Users;

public sealed class UserListQueryParameters
{
    public string? Search { get; init; }
    public string? Role { get; init; }
    public bool? IsActive { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

