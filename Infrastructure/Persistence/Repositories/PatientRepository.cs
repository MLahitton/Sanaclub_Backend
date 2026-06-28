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

    public async Task<Patient?> GetByIdForUpdateAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Patients
            .SingleOrDefaultAsync(x => x.Id == patientId, cancellationToken);
    }

    public async Task<IReadOnlyList<Patient>> SearchForClinicalSummaryAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var normalizedSearch = string.IsNullOrWhiteSpace(search)
            ? null
            : search.Trim();

        return await BuildSearchQuery(_context.Patients.AsNoTracking(), normalizedSearch, null, null)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ThenBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Patient>> ListAsync(
        string? search,
        bool? isActive,
        Guid? patientStatusId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = BuildSearchQuery(_context.Patients.AsNoTracking(), search, isActive, patientStatusId);

        return await query
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .ThenBy(x => x.IdentificationNumber)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        string? search,
        bool? isActive,
        Guid? patientStatusId,
        CancellationToken cancellationToken = default)
    {
        var query = BuildSearchQuery(_context.Patients.AsNoTracking(), search, isActive, patientStatusId);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsByIdentificationExcludingPatientAsync(
        Guid patientId,
        Guid identificationTypeId,
        string identificationNumber,
        CancellationToken cancellationToken = default)
    {
        var trimmedIdentificationNumber = identificationNumber.Trim();

        return await _context.Patients
            .AsNoTracking()
            .AnyAsync(
                x => x.Id != patientId
                    && x.IdentificationTypeId == identificationTypeId
                    && x.IdentificationNumber == trimmedIdentificationNumber,
                cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Patient> BuildSearchQuery(
        IQueryable<Patient> query,
        string? search,
        bool? isActive,
        Guid? patientStatusId)
    {
        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        if (patientStatusId.HasValue && patientStatusId.Value != Guid.Empty)
        {
            query = query.Where(x => x.PatientStatusId == patientStatusId.Value);
        }

        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var searchTrimmed = search.Trim();
        var terms = searchTrimmed
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var term in terms)
        {
            var pattern = $"%{term}%";

            query = query.Where(x =>
                EF.Functions.ILike(x.IdentificationNumber, pattern) ||
                EF.Functions.ILike(x.FirstName, pattern) ||
                EF.Functions.ILike(x.LastName, pattern) ||
                EF.Functions.ILike(x.FirstName + " " + x.LastName, pattern) ||
                EF.Functions.ILike(x.LastName + " " + x.FirstName, pattern) ||
                (x.PhoneNumber != null && EF.Functions.ILike(x.PhoneNumber, pattern)) ||
                (x.Email != null && EF.Functions.ILike(x.Email, pattern)));
        }

        return query;
    }
}
