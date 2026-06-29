using MediatR;
using Sanaclub.Application.Appointments.Common;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;

namespace Sanaclub.Application.Appointments.Confirm;

public sealed class ConfirmAppointmentCommand : IRequest<AppointmentResponse>
{
    public Guid AppointmentId { get; init; }
    public Guid ConfirmedByUserId { get; init; }
}

public sealed class ConfirmAppointmentCommandHandler : IRequestHandler<ConfirmAppointmentCommand, AppointmentResponse>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ICatalogRepository _catalogRepository;

    public ConfirmAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        ICatalogRepository catalogRepository)
    {
        _appointmentRepository = appointmentRepository;
        _catalogRepository = catalogRepository;
    }

    public async Task<AppointmentResponse> Handle(
        ConfirmAppointmentCommand request,
        CancellationToken cancellationToken)
    {
        if (request.AppointmentId == Guid.Empty)
        {
            throw new AppValidationException("appointmentId", "El identificador de la cita es obligatorio.");
        }

        if (request.ConfirmedByUserId == Guid.Empty)
        {
            throw new AppValidationException("confirmedByUserId", "El usuario que confirma la cita es obligatorio.");
        }

        var appointment = await _appointmentRepository.GetResponseByIdAsync(
            request.AppointmentId,
            cancellationToken);

        if (appointment is null)
        {
            throw new NotFoundException("La cita seleccionada no existe.");
        }

        if (string.Equals(appointment.StatusCode, AppointmentConstants.CancelledStatusCode, StringComparison.OrdinalIgnoreCase))
        {
            throw new ConflictException("La cita cancelada no puede confirmarse.");
        }

        if (string.Equals(appointment.StatusCode, AppointmentConstants.ConfirmedStatusCode, StringComparison.OrdinalIgnoreCase))
        {
            return appointment;
        }

        var confirmedStatusId = await _catalogRepository.GetAppointmentStatusIdByCodeAsync(
            AppointmentConstants.ConfirmedStatusCode,
            cancellationToken);

        if (confirmedStatusId is null)
        {
            throw new NotFoundException("El estado de cita CONFIRMED no fue encontrado.");
        }

        await _appointmentRepository.ConfirmAsync(
            request.AppointmentId,
            confirmedStatusId.Value,
            request.ConfirmedByUserId,
            cancellationToken);

        var confirmedAppointment = await _appointmentRepository.GetResponseByIdAsync(
            request.AppointmentId,
            cancellationToken);

        if (confirmedAppointment is null)
        {
            throw new NotFoundException("La cita seleccionada no existe.");
        }

        return confirmedAppointment;
    }
}
