namespace Sanaclub.Application.Appointments.ListTherapists;

public sealed class AppointmentTherapistResponse
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
