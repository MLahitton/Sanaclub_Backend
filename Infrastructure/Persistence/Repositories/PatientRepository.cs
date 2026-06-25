using Microsoft.EntityFrameworkCore;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Domain.Patients;
using Sanaclub.Infrastructure.Persistence;

namespace Sanaclub.Infrastructure.Persistence.Repositories;

public sealed class PatientRepository : IPatientRepository
{
    private readonly SanaclubDbContext _context;

    public PatientRepository(SanaclubDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsByIdentificationAsync(
        Guid identificationTypeId,
        string identificationNumber,
        CancellationToken cancellationToken = default)
    {
        var trimmedIdentificationNumber = identificationNumber.Trim();

        return await _context.Patients
            .AsNoTracking()
            .AnyAsync(
                x => x.IdentificationTypeId == identificationTypeId && x.IdentificationNumber == trimmedIdentificationNumber,
                cancellationToken);
    }

    public async Task AddAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        await _context.Patients.AddAsync(patient, cancellationToken);
    }

    public async Task<Patient?> GetByIdAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await _context.Patients
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == patientId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Patient>> ListAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = BuildSearchQuery(_context.Patients.AsNoTracking(), search);

        return await query
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(string? search, CancellationToken cancellationToken = default)
    {
        var query = BuildSearchQuery(_context.Patients.AsNoTracking(), search);
        return await query.CountAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Patient> BuildSearchQuery(
        IQueryable<Patient> query,
        string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var searchTrimmed = search.Trim();

        return query.Where(x =>
            x.IdentificationNumber.Contains(searchTrimmed) ||
            x.FirstName.Contains(searchTrimmed) ||
            x.LastName.Contains(searchTrimmed));
    }
}
