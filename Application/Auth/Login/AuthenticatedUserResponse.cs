using System;

namespace Sanaclub.Application.Auth.Login;

public sealed class AuthenticatedUserResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public bool MustChangePassword { get; init; }
    public IReadOnlyCollection<string> Roles { get; init; } = Array.Empty<string>();
    public IReadOnlyCollection<string> Permissions { get; init; } = Array.Empty<string>();
}
