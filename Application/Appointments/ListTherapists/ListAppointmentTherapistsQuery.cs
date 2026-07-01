using MediatR;
using Sanaclub.Application.Common.Abstractions;

namespace Sanaclub.Application.Appointments.ListTherapists;

public sealed class ListAppointmentTherapistsQuery
    : IRequest<IReadOnlyCollection<AppointmentTherapistResponse>>
{
}

public sealed class ListAppointmentTherapistsQueryHandler
    : IRequestHandler<ListAppointmentTherapistsQuery, IReadOnlyCollection<AppointmentTherapistResponse>>
{
    private readonly IAuthRepository _authRepository;

    public ListAppointmentTherapistsQueryHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<IReadOnlyCollection<AppointmentTherapistResponse>> Handle(
        ListAppointmentTherapistsQuery request,
        CancellationToken cancellationToken)
    {
        return await _authRepository.ListActiveTherapistsAsync(cancellationToken);
    }
}
