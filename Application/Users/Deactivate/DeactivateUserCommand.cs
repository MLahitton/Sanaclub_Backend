using MediatR;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Application.Users.Common;

namespace Sanaclub.Application.Users.Deactivate;

public sealed class DeactivateUserCommand : IRequest<UserResponse>
{
    public Guid UserId { get; init; }
    public Guid DeactivatedByUserId { get; init; }
}

public sealed class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, UserResponse>
{
    private const string UserIdField = "userId";
    private const string DeactivatedByUserIdField = "deactivatedByUserId";

    private readonly IAuthRepository _authRepository;

    public DeactivateUserCommandHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<UserResponse> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty)
        {
            throw new AppValidationException(
                UserIdField,
                "El identificador de usuario es obligatorio.");
        }

        if (request.DeactivatedByUserId == Guid.Empty)
        {
            throw new AppValidationException(
                DeactivatedByUserIdField,
                "El identificador del usuario que desactiva es obligatorio.");
        }

        if (request.UserId == request.DeactivatedByUserId)
        {
            throw new ConflictException(UserManagementConstants.UsersDeactivateSelfConflictMessage);
        }

        var user = await _authRepository.GetUserByIdForUpdateAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("El usuario no fue encontrado.");
        }

        await EnsureNotRemovingLastActiveAdmin(request.UserId, request.DeactivatedByUserId, cancellationToken);

        if (!user.IsActive)
        {
            return UserResponse.From(user, await _authRepository.GetActiveRoleByUserIdAsync(request.UserId, cancellationToken));
        }

        var activeRefreshTokens = await _authRepository.GetActiveRefreshTokensByUserIdAsync(
            request.UserId,
            cancellationToken);

        foreach (var refreshToken in activeRefreshTokens)
        {
            refreshToken.Revoke(
                reason: UserManagementConstants.UsersDeactivateReason,
                revokedByUserId: request.DeactivatedByUserId);
        }

        user.Deactivate(UserManagementConstants.UsersDeactivateReason);
        user.MarkAsUpdated(request.DeactivatedByUserId);
        await _authRepository.SaveChangesAsync(cancellationToken);

        var role = await _authRepository.GetActiveRoleByUserIdAsync(request.UserId, cancellationToken);
        return UserResponse.From(user, role);
    }

    private async Task EnsureNotRemovingLastActiveAdmin(
        Guid userId,
        Guid deactivatedByUserId,
        CancellationToken cancellationToken)
    {
        var adminRole = await _authRepository.GetRoleByCodeAsync(UserManagementConstants.AdminRoleCode, cancellationToken);
        if (adminRole is null)
        {
            return;
        }

        var activeRoles = await _authRepository.GetActiveUserRolesByUserIdAsync(userId, cancellationToken);
        if (!activeRoles.Any(role => role.IsActive && role.RoleId == adminRole.Id))
        {
            return;
        }

        if (userId == deactivatedByUserId)
        {
            return;
        }

        var activeAdmins = await _authRepository.CountActiveUsersByRoleCodeAsync(
            UserManagementConstants.AdminRoleCode,
            cancellationToken);

        if (activeAdmins <= 1)
        {
            throw new ConflictException(UserManagementConstants.LastActiveAdminConflictMessage);
        }
    }
}

