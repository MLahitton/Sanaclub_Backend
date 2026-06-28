using Microsoft.EntityFrameworkCore;
using Sanaclub.Application.Common.Abstractions;
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

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}

