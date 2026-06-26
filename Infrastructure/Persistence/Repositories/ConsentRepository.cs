using Microsoft.EntityFrameworkCore;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Domain.Consents;

namespace Sanaclub.Infrastructure.Persistence.Repositories;

public sealed class ConsentRepository : IConsentRepository
{
    private readonly SanaclubDbContext _context;

    public ConsentRepository(SanaclubDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(InformedConsent consent, CancellationToken cancellationToken = default)
    {
        await _context.InformedConsents.AddAsync(consent, cancellationToken);
    }

    public async Task<InformedConsent?> GetByIdAsync(
        Guid consentId,
        CancellationToken cancellationToken = default)
    {
        return await _context.InformedConsents
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == consentId, cancellationToken);
    }

    public async Task<InformedConsent?> GetByIdForUpdateAsync(
        Guid consentId,
        CancellationToken cancellationToken = default)
    {
        return await _context.InformedConsents
            .SingleOrDefaultAsync(x => x.Id == consentId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<InformedConsent>> ListByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        return await _context.InformedConsents
            .AsNoTracking()
            .Where(x => x.PatientId == patientId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
