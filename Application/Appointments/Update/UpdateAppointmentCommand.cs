using MediatR;
using Sanaclub.Application.Appointments.Common;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;

namespace Sanaclub.Application.Appointments.Update;

public sealed class UpdateAppointmentCommand : IRequest<AppointmentResponse>
{
    public Guid AppointmentId { get; init; }
    public Guid TherapistUserId { get; init; }
    public DateOnly? AppointmentDate { get; init; }
    public TimeOnly? StartTime { get; init; }
    public string? Notes { get; init; }
    public Guid UpdatedByUserId { get; init; }
}

public sealed class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand, AppointmentResponse>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAuthRepository _authRepository;

    public UpdateAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IAuthRepository authRepository)
    {
        _appointmentRepository = appointmentRepository;
        _authRepository = authRepository;
    }

    public async Task<AppointmentResponse> Handle(
        UpdateAppointmentCommand request,
        CancellationToken cancellationToken)
    {
        ValidateRequest(request);

        var currentAppointment = await _appointmentRepository.GetResponseByIdAsync(
            request.AppointmentId,
            cancellationToken);

        if (currentAppointment is null)
        {
            throw new NotFoundException("La cita seleccionada no existe.");
        }

        if (string.Equals(
                currentAppointment.StatusCode,
                AppointmentConstants.CancelledStatusCode,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ConflictException("La cita cancelada no puede editarse.");
        }

        var appointmentDate = request.AppointmentDate!.Value;
        var startTime = request.StartTime!.Value;
        var endDateTime = appointmentDate.ToDateTime(startTime)
            .AddMinutes(AppointmentConstants.AppointmentDurationMinutes);

        if (DateOnly.FromDateTime(endDateTime) != appointmentDate)
        {
            throw new ConflictException("La hora de la cita cruza al día siguiente y no es válida.");
        }

        var endTime = TimeOnly.FromDateTime(endDateTime);

        await ValidateTherapistAsync(request.TherapistUserId, cancellationToken);

        var hasConflict = await _appointmentRepository.HasTherapistScheduleConflictAsync(
            request.TherapistUserId,
            appointmentDate,
            startTime,
            endTime,
            request.AppointmentId,
            cancellationToken);

        if (hasConflict)
        {
            throw new ConflictException("El horario seleccionado no está disponible para la terapeuta.");
        }

        await _appointmentRepository.UpdateScheduleAsync(
            request.AppointmentId,
            request.TherapistUserId,
            appointmentDate,
            startTime,
            endTime,
            request.Notes,
            request.UpdatedByUserId,
            cancellationToken);

        var updatedAppointment = await _appointmentRepository.GetResponseByIdAsync(
            request.AppointmentId,
            cancellationToken);

        if (updatedAppointment is null)
        {
            throw new NotFoundException("La cita seleccionada no existe.");
        }

        return updatedAppointment;
    }

    private static void ValidateRequest(UpdateAppointmentCommand request)
    {
        if (request.AppointmentId == Guid.Empty)
        {
            throw new AppValidationException("appointmentId", "El identificador de la cita es obligatorio.");
        }

        if (request.TherapistUserId == Guid.Empty)
        {
            throw new AppValidationException("therapistUserId", "La terapeuta es obligatoria.");
        }

        if (request.AppointmentDate is null || request.AppointmentDate.Value == default)
        {
            throw new AppValidationException("appointmentDate", "La fecha de la cita es obligatoria.");
        }

        if (request.StartTime is null)
        {
            throw new AppValidationException("startTime", "La hora de inicio de la cita es obligatoria.");
        }

        if (request.UpdatedByUserId == Guid.Empty)
        {
            throw new AppValidationException("updatedByUserId", "El usuario que actualiza la cita es obligatorio.");
        }
    }

    private async Task ValidateTherapistAsync(
        Guid therapistUserId,
        CancellationToken cancellationToken)
    {
        var therapist = await _authRepository.GetUserByIdAsync(therapistUserId, cancellationToken);
        if (therapist is null)
        {
            throw new NotFoundException("La terapeuta seleccionada no existe.");
        }

        if (!therapist.IsActive)
        {
            throw new ConflictException("La terapeuta seleccionada no está activa.");
        }

        var roleCodes = await _authRepository.GetActiveRoleCodesByUserIdAsync(
            therapistUserId,
            cancellationToken);

        if (!roleCodes.Any(roleCode =>
                string.Equals(roleCode, AppointmentConstants.TherapistRoleCode, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ConflictException("La cita solo puede asignarse a un usuario con rol THERAPIST.");
        }
    }
}
