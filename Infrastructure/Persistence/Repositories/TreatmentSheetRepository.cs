using Microsoft.EntityFrameworkCore;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.TreatmentSheets.PendingMedicalIndication;
using Sanaclub.Domain.TreatmentSheets;
using Sanaclub.Infrastructure.Persistence;

namespace Sanaclub.Infrastructure.Persistence.Repositories;

public sealed class TreatmentSheetRepository : ITreatmentSheetRepository
{
    private readonly SanaclubDbContext _context;

    public TreatmentSheetRepository(SanaclubDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TreatmentSheet treatmentSheet, CancellationToken cancellationToken = default)
    {
        await _context.Set<TreatmentSheet>().AddAsync(treatmentSheet, cancellationToken);
    }

    public async Task<TreatmentSheet?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<TreatmentSheet>()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<TreatmentSheet?> GetByIdForUpdateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<TreatmentSheet>()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<TreatmentSheet>> ListByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<TreatmentSheet>()
            .AsNoTracking()
            .Where(x => x.PatientId == patientId && x.IsActive)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<PendingTreatmentSheetResponse>> ListPendingMedicalIndicationAsync(
        Guid draftStatusId,
        string? search,
        DateOnly? fromDate,
        DateOnly? toDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await BuildPendingMedicalIndicationResponseQuery(
                BuildPendingMedicalIndicationQuery(draftStatusId, search, fromDate, toDate))
            .OrderBy(x => x.CreatedAtUtc)
            .ThenBy(x => x.ConsultationDate)
            .ThenBy(x => x.PatientFullName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountPendingMedicalIndicationAsync(
        Guid draftStatusId,
        string? search,
        DateOnly? fromDate,
        DateOnly? toDate,
        CancellationToken cancellationToken = default)
    {
        return await BuildPendingMedicalIndicationQuery(draftStatusId, search, fromDate, toDate)
            .CountAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<TreatmentSheet> BuildPendingMedicalIndicationQuery(
        Guid draftStatusId,
        string? search,
        DateOnly? fromDate,
        DateOnly? toDate)
    {
        var query = _context.Set<TreatmentSheet>()
            .AsNoTracking()
            .Where(x => x.IsActive && x.TreatmentStatusId == draftStatusId);

        if (fromDate.HasValue)
        {
            var fromDateTime = fromDate.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(x =>
                (x.ConsultationDate.HasValue && x.ConsultationDate.Value >= fromDate.Value)
                || (!x.ConsultationDate.HasValue && x.CreatedAtUtc >= fromDateTime));
        }

        if (toDate.HasValue)
        {
            var toDateTimeExclusive = toDate.Value.AddDays(1).ToDateTime(TimeOnly.MinValue);
            query = query.Where(x =>
                (x.ConsultationDate.HasValue && x.ConsultationDate.Value <= toDate.Value)
                || (!x.ConsultationDate.HasValue && x.CreatedAtUtc < toDateTimeExclusive));
        }

        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var terms = search.Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var term in terms)
        {
            var pattern = $"%{term}%";
            query = query.Where(treatmentSheet =>
                (treatmentSheet.TreatmentNumber != null
                    && EF.Functions.ILike(treatmentSheet.TreatmentNumber, pattern))
                || _context.Patients.AsNoTracking().Any(patient =>
                    patient.Id == treatmentSheet.PatientId
                    && (EF.Functions.ILike(patient.FirstName, pattern)
                        || EF.Functions.ILike(patient.LastName, pattern)
                        || EF.Functions.ILike(patient.FirstName + " " + patient.LastName, pattern)
                        || EF.Functions.ILike(patient.LastName + " " + patient.FirstName, pattern)
                        || EF.Functions.ILike(patient.IdentificationNumber, pattern))));
        }

        return query;
    }

    private IQueryable<PendingTreatmentSheetResponse> BuildPendingMedicalIndicationResponseQuery(
        IQueryable<TreatmentSheet> query)
    {
        return from treatmentSheet in query
               join patient in _context.Patients.AsNoTracking()
                   on treatmentSheet.PatientId equals patient.Id
               join status in _context.TreatmentStatuses.AsNoTracking()
                   on treatmentSheet.TreatmentStatusId equals status.Id
               select new PendingTreatmentSheetResponse
               {
                   Id = treatmentSheet.Id,
                   PatientId = treatmentSheet.PatientId,
                   PatientFullName = patient.FirstName + " " + patient.LastName,
                   PatientIdentificationNumber = patient.IdentificationNumber,
                   TreatmentNumber = treatmentSheet.TreatmentNumber,
                   ConsultationDate = treatmentSheet.ConsultationDate,
                   EpsDiagnosis = treatmentSheet.EpsTreatingDoctorDiagnosis,
                   ReferredClinicalHistory = treatmentSheet.ReferredClinicalHistory,
                   TreatmentStatusId = treatmentSheet.TreatmentStatusId,
                   StatusCode = status.Code,
                   StatusName = status.Name,
                   CreatedAtUtc = treatmentSheet.CreatedAtUtc,
                   UpdatedAtUtc = treatmentSheet.UpdatedAtUtc
               };
    }
}
