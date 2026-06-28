using Microsoft.EntityFrameworkCore;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Domain.Documents;
using Sanaclub.Infrastructure.Persistence;

namespace Sanaclub.Infrastructure.Persistence.Repositories;

public sealed class GeneratedDocumentRepository : IGeneratedDocumentRepository
{
    private readonly SanaclubDbContext _context;

    public GeneratedDocumentRepository(SanaclubDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(GeneratedDocument document, CancellationToken cancellationToken = default)
    {
        await _context.GeneratedDocuments.AddAsync(document, cancellationToken);
    }

    public async Task<GeneratedDocument?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.GeneratedDocuments
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<GeneratedDocument>> ListByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        return await _context.GeneratedDocuments
            .AsNoTracking()
            .Where(x => x.PatientId == patientId && x.IsActive)
            .OrderByDescending(x => x.GeneratedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
