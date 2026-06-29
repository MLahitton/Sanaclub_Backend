using MediatR;
using Sanaclub.Application.Appointments.Common;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;

namespace Sanaclub.Application.Appointments.Cancel;

public sealed class CancelAppointmentCommand : IRequest<AppointmentResponse>
{
    public Guid AppointmentId { get; init; }
    public string? Notes { get; init; }
    public Guid CancelledByUserId { get; init; }
}

public sealed class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, AppointmentResponse>
{
    private const int NotesMaxLength = 2000;

    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ICatalogRepository _catalogRepository;

    public CancelAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        ICatalogRepository catalogRepository)
    {
        _appointmentRepository = appointmentRepository;
        _catalogRepository = catalogRepository;
    }

    public async Task<AppointmentResponse> Handle(
        CancelAppointmentCommand request,
        CancellationToken cancellationToken)
    {
        if (request.AppointmentId == Guid.Empty)
        {
            throw new AppValidationException("appointmentId", "El identificador de la cita es obligatorio.");
        }

        if (request.CancelledByUserId == Guid.Empty)
        {
            throw new AppValidationException("cancelledByUserId", "El usuario que cancela la cita es obligatorio.");
        }

        var trimmedNotes = string.IsNullOrWhiteSpace(request.Notes)
            ? null
            : request.Notes.Trim();

        if (trimmedNotes is not null && trimmedNotes.Length > NotesMaxLength)
        {
            throw new AppValidationException("notes", "Las notas de la cita no pueden superar 2000 caracteres.");
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
            return appointment;
        }

        var cancelledStatusId = await _catalogRepository.GetAppointmentStatusIdByCodeAsync(
            AppointmentConstants.CancelledStatusCode,
            cancellationToken);

        if (cancelledStatusId is null)
        {
            throw new NotFoundException("El estado de cita CANCELLED no fue encontrado.");
        }

        await _appointmentRepository.CancelAsync(
            request.AppointmentId,
            cancelledStatusId.Value,
            request.CancelledByUserId,
            trimmedNotes,
            cancellationToken);

        var cancelledAppointment = await _appointmentRepository.GetResponseByIdAsync(
            request.AppointmentId,
            cancellationToken);

        if (cancelledAppointment is null)
        {
            throw new NotFoundException("La cita seleccionada no existe.");
        }

        return cancelledAppointment;
    }
}
