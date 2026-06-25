using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;

namespace Sanaclub.Application.Auth.Me;

public sealed class GetCurrentUserQuery : IRequest<CurrentUserResponse>
{
    public Guid UserId { get; init; }
}

public sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserResponse>
{
    private const string InvalidAuthenticatedUserMessage = "Authenticated user is not valid.";

    private readonly IAuthRepository _authRepository;

    public GetCurrentUserQueryHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<CurrentUserResponse> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty)
        {
            throw new ForbiddenException(InvalidAuthenticatedUserMessage);
        }

        var user = await _authRepository.GetUserByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new ForbiddenException(InvalidAuthenticatedUserMessage);
        }

        if (!user.IsActive)
        {
            throw new ForbiddenException(InvalidAuthenticatedUserMessage);
        }

        var roles = await _authRepository.GetActiveRoleCodesByUserIdAsync(request.UserId, cancellationToken);
        var permissions = await _authRepository.GetActivePermissionCodesByUserIdAsync(request.UserId, cancellationToken);

        return new CurrentUserResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            MustChangePassword = user.MustChangePassword,
            Roles = roles,
            Permissions = permissions
        };
    }
}
