using Microsoft.EntityFrameworkCore;
using Sanaclub.Application.Appointments.Common;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Domain.Appointments;

namespace Sanaclub.Infrastructure.Persistence.Repositories;

public sealed class AppointmentRepository : IAppointmentRepository
{
    private readonly SanaclubDbContext _context;

    public AppointmentRepository(SanaclubDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Appointment appointment,
        CancellationToken cancellationToken = default)
    {
        await _context.Appointments.AddAsync(appointment, cancellationToken);
    }

    public async Task<Appointment?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<AppointmentResponse?> GetResponseByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await BuildResponseQuery(_context.Appointments.AsNoTracking())
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<AppointmentResponse>> ListAsync(
        AppointmentListFilters filters,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await BuildResponseQuery(ApplyFilters(_context.Appointments.AsNoTracking(), filters))
            .OrderBy(x => x.AppointmentDate)
            .ThenBy(x => x.StartTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        AppointmentListFilters filters,
        CancellationToken cancellationToken = default)
    {
        return await ApplyFilters(_context.Appointments.AsNoTracking(), filters)
            .CountAsync(cancellationToken);
    }

    public async Task<bool> HasTherapistScheduleConflictAsync(
        Guid therapistUserId,
        DateOnly appointmentDate,
        TimeOnly startTime,
        TimeOnly endTime,
        Guid? excludedAppointmentId = null,
        CancellationToken cancellationToken = default)
    {
        var query = from appointment in _context.Appointments.AsNoTracking()
                    join status in _context.AppointmentStatuses.AsNoTracking()
                        on appointment.AppointmentStatusId equals status.Id
                    where appointment.TherapistUserId == therapistUserId
                          && appointment.AppointmentDate == appointmentDate
                          && appointment.IsActive
                          && (status.Code == AppointmentConstants.ScheduledStatusCode
                              || status.Code == AppointmentConstants.ConfirmedStatusCode)
                          && startTime < appointment.EndTime
                          && endTime > appointment.StartTime
                    select appointment;

        if (excludedAppointmentId.HasValue && excludedAppointmentId.Value != Guid.Empty)
        {
            query = query.Where(x => x.Id != excludedAppointmentId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<int> UpdateScheduleAsync(
        Guid appointmentId,
        Guid therapistUserId,
        DateOnly appointmentDate,
        TimeOnly startTime,
        TimeOnly endTime,
        string? notes,
        Guid updatedByUserId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var normalizedNotes = string.IsNullOrWhiteSpace(notes)
            ? null
            : notes.Trim();

        return await _context.Appointments
            .Where(x => x.Id == appointmentId)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(x => x.TherapistUserId, therapistUserId)
                    .SetProperty(x => x.AppointmentDate, appointmentDate)
                    .SetProperty(x => x.StartTime, startTime)
                    .SetProperty(x => x.EndTime, endTime)
                    .SetProperty(x => x.Notes, normalizedNotes)
                    .SetProperty(x => x.UpdatedAtUtc, now)
                    .SetProperty(x => x.UpdatedByUserId, (Guid?)updatedByUserId),
                cancellationToken);
    }

    public async Task<int> ConfirmAsync(
        Guid appointmentId,
        Guid confirmedStatusId,
        Guid confirmedByUserId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await _context.Appointments
            .Where(x => x.Id == appointmentId)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(x => x.AppointmentStatusId, confirmedStatusId)
                    .SetProperty(x => x.ConfirmedAtUtc, now)
                    .SetProperty(x => x.ConfirmedByUserId, (Guid?)confirmedByUserId)
                    .SetProperty(x => x.UpdatedAtUtc, now)
                    .SetProperty(x => x.UpdatedByUserId, (Guid?)confirmedByUserId),
                cancellationToken);
    }

    public async Task<int> CancelAsync(
        Guid appointmentId,
        Guid cancelledStatusId,
        Guid cancelledByUserId,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var normalizedNotes = string.IsNullOrWhiteSpace(notes)
            ? null
            : notes.Trim();

        if (normalizedNotes is null)
        {
            return await _context.Appointments
                .Where(x => x.Id == appointmentId)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(x => x.AppointmentStatusId, cancelledStatusId)
                        .SetProperty(x => x.CancelledAtUtc, now)
                        .SetProperty(x => x.CancelledByUserId, (Guid?)cancelledByUserId)
                        .SetProperty(x => x.UpdatedAtUtc, now)
                        .SetProperty(x => x.UpdatedByUserId, (Guid?)cancelledByUserId),
                    cancellationToken);
        }

        return await _context.Appointments
            .Where(x => x.Id == appointmentId)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(x => x.AppointmentStatusId, cancelledStatusId)
                    .SetProperty(x => x.CancelledAtUtc, now)
                    .SetProperty(x => x.CancelledByUserId, (Guid?)cancelledByUserId)
                    .SetProperty(x => x.Notes, normalizedNotes)
                    .SetProperty(x => x.UpdatedAtUtc, now)
                    .SetProperty(x => x.UpdatedByUserId, (Guid?)cancelledByUserId),
                cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<Appointment> ApplyFilters(
        IQueryable<Appointment> query,
        AppointmentListFilters filters)
    {
        if (!filters.IncludeCancelled)
        {
            query = from appointment in query
                    join status in _context.AppointmentStatuses.AsNoTracking()
                        on appointment.AppointmentStatusId equals status.Id
                    where status.Code != AppointmentConstants.CancelledStatusCode
                    select appointment;
        }

        if (filters.Date.HasValue)
        {
            query = query.Where(x => x.AppointmentDate == filters.Date.Value);
        }
        else
        {
            if (filters.FromDate.HasValue)
            {
                query = query.Where(x => x.AppointmentDate >= filters.FromDate.Value);
            }

            if (filters.ToDate.HasValue)
            {
                query = query.Where(x => x.AppointmentDate <= filters.ToDate.Value);
            }
        }

        if (filters.TherapistUserId.HasValue && filters.TherapistUserId.Value != Guid.Empty)
        {
            query = query.Where(x => x.TherapistUserId == filters.TherapistUserId.Value);
        }

        if (filters.PatientId.HasValue && filters.PatientId.Value != Guid.Empty)
        {
            query = query.Where(x => x.PatientId == filters.PatientId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.ClinicalReferenceType))
        {
            query = query.Where(x => x.ClinicalReferenceType == filters.ClinicalReferenceType);
        }

        if (filters.ClinicalReferenceId.HasValue && filters.ClinicalReferenceId.Value != Guid.Empty)
        {
            query = query.Where(x => x.ClinicalReferenceId == filters.ClinicalReferenceId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.Status))
        {
            query = from appointment in query
                    join status in _context.AppointmentStatuses.AsNoTracking()
                        on appointment.AppointmentStatusId equals status.Id
                    where status.Code == filters.Status
                    select appointment;
        }

        return query;
    }

    private IQueryable<AppointmentResponse> BuildResponseQuery(IQueryable<Appointment> appointments)
    {
        return from appointment in appointments
               join patient in _context.Patients.AsNoTracking()
                   on appointment.PatientId equals patient.Id
               join treatmentSheet in _context.TreatmentSheets.AsNoTracking()
                   on appointment.TreatmentSheetId equals treatmentSheet.Id
               join therapist in _context.Users.AsNoTracking()
                   on appointment.TherapistUserId equals therapist.Id
               join status in _context.AppointmentStatuses.AsNoTracking()
                   on appointment.AppointmentStatusId equals status.Id
               join evolutionSheet in _context.EvolutionSheets.AsNoTracking()
                   on appointment.ClinicalReferenceId equals evolutionSheet.Id into evolutionSheets
               from evolutionSheet in evolutionSheets.DefaultIfEmpty()
               select new AppointmentResponse
               {
                   Id = appointment.Id,
                   PatientId = appointment.PatientId,
                   PatientFullName = patient.FirstName + " " + patient.LastName,
                   PatientIdentificationNumber = patient.IdentificationNumber,
                   TreatmentSheetId = appointment.TreatmentSheetId,
                   TreatmentNumber = treatmentSheet.TreatmentNumber,
                   ClinicalReferenceType = appointment.ClinicalReferenceType,
                   ClinicalReferenceId = appointment.ClinicalReferenceId,
                   ClinicalReferenceLabel = appointment.ClinicalReferenceType == AppointmentConstants.EvolutionSheetClinicalReferenceType
                       ? "Hoja de evolución - " + (evolutionSheet != null ? evolutionSheet.TherapyNumber : null)
                       : "Hoja de tratamiento - " + treatmentSheet.TreatmentNumber,
                   TherapistUserId = appointment.TherapistUserId,
                   TherapistFullName = therapist.FullName,
                   AppointmentDate = appointment.AppointmentDate,
                   StartTime = appointment.StartTime,
                   EndTime = appointment.EndTime,
                   AppointmentStatusId = appointment.AppointmentStatusId,
                   StatusCode = status.Code,
                   StatusName = status.Name,
                   Notes = appointment.Notes,
                   ScheduledByUserId = appointment.ScheduledByUserId,
                   ConfirmedAtUtc = appointment.ConfirmedAtUtc,
                   ConfirmedByUserId = appointment.ConfirmedByUserId,
                   CancelledAtUtc = appointment.CancelledAtUtc,
                   CancelledByUserId = appointment.CancelledByUserId,
                   IsActive = appointment.IsActive,
                   CreatedAtUtc = appointment.CreatedAtUtc,
                   UpdatedAtUtc = appointment.UpdatedAtUtc
               };
    }
}
