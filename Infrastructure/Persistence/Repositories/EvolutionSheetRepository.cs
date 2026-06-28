using Microsoft.EntityFrameworkCore;
using Sanaclub.Application.Common.Abstractions;
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

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
