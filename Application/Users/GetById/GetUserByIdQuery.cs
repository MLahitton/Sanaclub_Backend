using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.Users.Common;

namespace Sanaclub.Application.Users.GetById;

public sealed class GetUserByIdQuery : IRequest<UserResponse>
{
    public Guid UserId { get; init; }
}

public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserResponse>
{
    private readonly IAuthRepository _authRepository;

    public GetUserByIdQueryHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<UserResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty)
        {
            throw new AppValidationException(
                "id",
                "El identificador de usuario es obligatorio.");
        }

        var user = await _authRepository.GetUserByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("La persona usuaria seleccionada no existe.");
        }

        var role = await _authRepository.GetActiveRoleByUserIdAsync(request.UserId, cancellationToken);
        return UserResponse.From(user, role);
    }
}
