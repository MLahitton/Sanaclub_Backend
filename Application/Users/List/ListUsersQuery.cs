using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Models;
using Sanaclub.Application.Users.Common;

namespace Sanaclub.Application.Users.List;

public sealed class ListUsersQuery : IRequest<PaginatedResult<UserResponse>>
{
    public string? Search { get; init; }
    public string? Role { get; init; }
    public bool? IsActive { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public sealed class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, PaginatedResult<UserResponse>>
{
    private const int MaxPageSize = 100;
    private const int MinPageSize = 1;
    private const int MinPageNumber = 1;

    private readonly IAuthRepository _authRepository;

    public ListUsersQueryHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<PaginatedResult<UserResponse>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < MinPageNumber
            ? MinPageNumber
            : request.PageNumber;

        var pageSize = request.PageSize < MinPageSize
            ? MinPageSize
            : request.PageSize;

        if (pageSize > MaxPageSize)
        {
            pageSize = MaxPageSize;
        }

        var normalizedSearch = string.IsNullOrWhiteSpace(request.Search)
            ? null
            : request.Search.Trim();

        var normalizedRole = string.IsNullOrWhiteSpace(request.Role)
            ? null
            : request.Role.Trim().ToUpperInvariant();

        var totalCount = await _authRepository.CountUsersAsync(
            normalizedSearch,
            request.IsActive,
            normalizedRole,
            cancellationToken);

        var users = await _authRepository.ListUsersAsync(
            normalizedSearch,
            request.IsActive,
            normalizedRole,
            pageNumber,
            pageSize,
            cancellationToken);

        var items = new List<UserResponse>(users.Count);
        foreach (var user in users)
        {
            var role = await _authRepository.GetActiveRoleByUserIdAsync(user.Id, cancellationToken);
            items.Add(UserResponse.From(user, role));
        }

        return new PaginatedResult<UserResponse>(
            items,
            pageNumber,
            pageSize,
            totalCount);
    }
}

