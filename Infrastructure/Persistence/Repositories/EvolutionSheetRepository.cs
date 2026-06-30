using Microsoft.EntityFrameworkCore;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.EvolutionSheets.PendingNewIndications;
using Sanaclub.Domain.EvolutionSheets;
using Sanaclub.Infrastructure.Persistence;

namespace Sanaclub.Infrastructure.Persistence.Repositories;

public sealed class EvolutionSheetRepository : IEvolutionSheetRepository
{
    private readonly SanaclubDbContext _context;

    public EvolutionSheetRepository(SanaclubDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(EvolutionSheet evolutionSheet, CancellationToken cancellationToken = default)
    {
        await _context.Set<EvolutionSheet>()
            .AddAsync(evolutionSheet, cancellationToken);
    }

    public async Task<EvolutionSheet?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<EvolutionSheet>()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<EvolutionSheet?> GetByIdForUpdateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<EvolutionSheet>()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<EvolutionSheet>> ListByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<EvolutionSheet>()
            .AsNoTracking()
            .Where(x => x.PatientId == patientId && x.IsActive)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<PendingEvolutionSheetResponse>> ListPendingNewIndicationsAsync(
        Guid draftStatusId,
        string? search,
        DateOnly? fromDate,
        DateOnly? toDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await BuildPendingNewIndicationsResponseQuery(
                BuildPendingNewIndicationsQuery(draftStatusId, search, fromDate, toDate))
            .OrderBy(x => x.CreatedAtUtc)
            .ThenBy(x => x.EvolutionDate)
            .ThenBy(x => x.PatientFullName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountPendingNewIndicationsAsync(
        Guid draftStatusId,
        string? search,
        DateOnly? fromDate,
        DateOnly? toDate,
        CancellationToken cancellationToken = default)
    {
        return await BuildPendingNewIndicationsQuery(draftStatusId, search, fromDate, toDate)
            .CountAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<EvolutionSheet> BuildPendingNewIndicationsQuery(
        Guid draftStatusId,
        string? search,
        DateOnly? fromDate,
        DateOnly? toDate)
    {
        var query = _context.Set<EvolutionSheet>()
            .AsNoTracking()
            .Where(x => x.IsActive
                        && x.EvolutionStatusId == draftStatusId
                        && (x.NewIndications == null || x.NewIndications == string.Empty)
                        && x.CompletedAtUtc == null
                        && x.CompletedByUserId == null);

        if (fromDate.HasValue)
        {
            var fromDateTime = fromDate.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(x =>
                (x.EvolutionDate.HasValue && x.EvolutionDate.Value >= fromDate.Value)
                || (!x.EvolutionDate.HasValue && x.CreatedAtUtc >= fromDateTime));
        }

        if (toDate.HasValue)
        {
            var toDateTimeExclusive = toDate.Value.AddDays(1).ToDateTime(TimeOnly.MinValue);
            query = query.Where(x =>
                (x.EvolutionDate.HasValue && x.EvolutionDate.Value <= toDate.Value)
                || (!x.EvolutionDate.HasValue && x.CreatedAtUtc < toDateTimeExclusive));
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
            query = query.Where(evolutionSheet =>
                (evolutionSheet.TherapyNumber != null
                    && EF.Functions.ILike(evolutionSheet.TherapyNumber, pattern))
                || (evolutionSheet.TherapyName != null
                    && EF.Functions.ILike(evolutionSheet.TherapyName, pattern))
                || _context.Patients.AsNoTracking().Any(patient =>
                    patient.Id == evolutionSheet.PatientId
                    && (EF.Functions.ILike(patient.FirstName, pattern)
                        || EF.Functions.ILike(patient.LastName, pattern)
                        || EF.Functions.ILike(patient.FirstName + " " + patient.LastName, pattern)
                        || EF.Functions.ILike(patient.LastName + " " + patient.FirstName, pattern)
                        || EF.Functions.ILike(patient.IdentificationNumber, pattern)))
                || _context.TreatmentSheets.AsNoTracking().Any(treatmentSheet =>
                    treatmentSheet.Id == evolutionSheet.TreatmentSheetId
                    && treatmentSheet.TreatmentNumber != null
                    && EF.Functions.ILike(treatmentSheet.TreatmentNumber, pattern)));
        }

        return query;
    }

    private IQueryable<PendingEvolutionSheetResponse> BuildPendingNewIndicationsResponseQuery(
        IQueryable<EvolutionSheet> query)
    {
        return from evolutionSheet in query
               join patient in _context.Patients.AsNoTracking()
                   on evolutionSheet.PatientId equals patient.Id
               join treatmentSheet in _context.TreatmentSheets.AsNoTracking()
                   on evolutionSheet.TreatmentSheetId equals treatmentSheet.Id
               join status in _context.EvolutionStatuses.AsNoTracking()
                   on evolutionSheet.EvolutionStatusId equals status.Id
               select new PendingEvolutionSheetResponse
               {
                   Id = evolutionSheet.Id,
                   PatientId = evolutionSheet.PatientId,
                   PatientFullName = patient.FirstName + " " + patient.LastName,
                   PatientIdentificationNumber = patient.IdentificationNumber,
                   TreatmentSheetId = evolutionSheet.TreatmentSheetId,
                   TreatmentNumber = treatmentSheet.TreatmentNumber,
                   TherapyNumber = evolutionSheet.TherapyNumber,
                   EvolutionDate = evolutionSheet.EvolutionDate,
                   EntryTime = evolutionSheet.EntryTime,
                   ExitTime = evolutionSheet.ExitTime,
                   AssignedStaffName = evolutionSheet.AssignedStaffName,
                   TherapyName = evolutionSheet.TherapyName,
                   EvolutionNotes = evolutionSheet.EvolutionNotes,
                   NewIndications = evolutionSheet.NewIndications,
                   EvolutionStatusId = evolutionSheet.EvolutionStatusId,
                   StatusCode = status.Code,
                   StatusName = status.Name,
                   CreatedAtUtc = evolutionSheet.CreatedAtUtc,
                   UpdatedAtUtc = evolutionSheet.UpdatedAtUtc
               };
    }
}
