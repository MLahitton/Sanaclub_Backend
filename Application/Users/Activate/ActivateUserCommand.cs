using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.Users.Common;

namespace Sanaclub.Application.Users.Activate;

public sealed class ActivateUserCommand : IRequest<UserResponse>
{
    public Guid UserId { get; init; }
    public Guid ActivatedByUserId { get; init; }
}

public sealed class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, UserResponse>
{
    private const string UserIdField = "userId";
    private const string ActivatedByUserIdField = "activatedByUserId";

    private readonly IAuthRepository _authRepository;

    public ActivateUserCommandHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<UserResponse> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty)
        {
            throw new AppValidationException(
                UserIdField,
                "El identificador de usuario es obligatorio.");
        }

        if (request.ActivatedByUserId == Guid.Empty)
        {
            throw new AppValidationException(
                ActivatedByUserIdField,
                "El identificador del usuario activador es obligatorio.");
        }

        var user = await _authRepository.GetUserByIdForUpdateAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("El usuario no fue encontrado.");
        }

        if (!user.IsActive)
        {
            user.Activate();
            user.MarkAsUpdated(request.ActivatedByUserId);
            await _authRepository.SaveChangesAsync(cancellationToken);
        }

        var role = await _authRepository.GetActiveRoleByUserIdAsync(request.UserId, cancellationToken);
        return UserResponse.From(user, role);
    }
}

