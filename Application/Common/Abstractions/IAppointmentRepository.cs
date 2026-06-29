using Sanaclub.Application.Appointments.Common;
using Sanaclub.Domain.Appointments;

namespace Sanaclub.Application.Common.Abstractions;

public interface IAppointmentRepository
{
    Task AddAsync(
        Appointment appointment,
        CancellationToken cancellationToken = default);

    Task<Appointment?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<AppointmentResponse?> GetResponseByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AppointmentResponse>> ListAsync(
        AppointmentListFilters filters,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        AppointmentListFilters filters,
        CancellationToken cancellationToken = default);

    Task<bool> HasTherapistScheduleConflictAsync(
        Guid therapistUserId,
        DateOnly appointmentDate,
        TimeOnly startTime,
        TimeOnly endTime,
        Guid? excludedAppointmentId = null,
        CancellationToken cancellationToken = default);

    Task<int> UpdateScheduleAsync(
        Guid appointmentId,
        Guid therapistUserId,
        DateOnly appointmentDate,
        TimeOnly startTime,
        TimeOnly endTime,
        string? notes,
        Guid updatedByUserId,
        CancellationToken cancellationToken = default);

    Task<int> ConfirmAsync(
        Guid appointmentId,
        Guid confirmedStatusId,
        Guid confirmedByUserId,
        CancellationToken cancellationToken = default);

    Task<int> CancelAsync(
        Guid appointmentId,
        Guid cancelledStatusId,
        Guid cancelledByUserId,
        string? notes,
        CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
