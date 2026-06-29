using Sanaclub.Domain.Auth;

namespace Sanaclub.Application.Users.Common;

public sealed class UserResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string FullName { get; init; } = string.Empty;
    public Guid? RoleId { get; init; }
    public string? RoleCode { get; init; }
    public string? RoleName { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }

    public static UserResponse From(User user, Role? role)
    {
        var firstName = ParseFirstName(user.FullName);
        var lastName = ParseLastName(user.FullName);

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = firstName,
            LastName = lastName,
            FullName = user.FullName,
            RoleId = role?.Id,
            RoleCode = role?.Code,
            RoleName = role?.Name,
            IsActive = user.IsActive,
            CreatedAtUtc = user.CreatedAtUtc,
            UpdatedAtUtc = user.UpdatedAtUtc
        };
    }

    private static string? ParseFirstName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return null;
        }

        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return parts.Length == 0 ? null : parts[0];
    }

    private static string? ParseLastName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return null;
        }

        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length <= 1)
        {
            return null;
        }

        return string.Join(" ", parts.Skip(1));
    }
}

