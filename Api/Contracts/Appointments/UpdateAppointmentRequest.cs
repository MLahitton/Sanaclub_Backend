namespace Sanaclub.Api.Contracts.Appointments;

public sealed class UpdateAppointmentRequest
{
    public Guid TherapistUserId { get; init; }
    public DateOnly? AppointmentDate { get; init; }
    public TimeOnly? StartTime { get; init; }
    public string? Notes { get; init; }
}
