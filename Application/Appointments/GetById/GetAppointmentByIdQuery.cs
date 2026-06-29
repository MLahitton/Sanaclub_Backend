using MediatR;
using Sanaclub.Application.Appointments.Common;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;

namespace Sanaclub.Application.Appointments.GetById;

public sealed class GetAppointmentByIdQuery : IRequest<AppointmentResponse>
{
    public Guid AppointmentId { get; init; }
}

public sealed class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, AppointmentResponse>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public GetAppointmentByIdQueryHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<AppointmentResponse> Handle(
        GetAppointmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.AppointmentId == Guid.Empty)
        {
            throw new AppValidationException("appointmentId", "El identificador de la cita es obligatorio.");
        }

        var appointment = await _appointmentRepository.GetResponseByIdAsync(
            request.AppointmentId,
            cancellationToken);

        if (appointment is null)
        {
            throw new NotFoundException("La cita no fue encontrada.");
        }

        return appointment;
    }
}
