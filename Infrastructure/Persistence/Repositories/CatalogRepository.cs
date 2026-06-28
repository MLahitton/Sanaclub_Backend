using Microsoft.EntityFrameworkCore;
using Sanaclub.Application.Catalogs.Common;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Infrastructure.Persistence;

namespace Sanaclub.Infrastructure.Persistence.Repositories;

public sealed class CatalogRepository : ICatalogRepository
{
    private readonly SanaclubDbContext _context;

    public CatalogRepository(SanaclubDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<CatalogItemResponse>> ListIdentificationTypesAsync(
        CancellationToken cancellationToken = default)
    {
        var items = await _context.IdentificationTypes
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .Select(x => new CatalogItemResponse
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);

        return items;
    }

    public async Task<IReadOnlyCollection<CatalogItemResponse>> ListGendersAsync(
        CancellationToken cancellationToken = default)
    {
        var items = await _context.Genders
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .Select(x => new CatalogItemResponse
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);

        return items;
    }

    public async Task<IReadOnlyCollection<CatalogItemResponse>> ListCivilStatusesAsync(
        CancellationToken cancellationToken = default)
    {
        var items = await _context.CivilStatuses
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .Select(x => new CatalogItemResponse
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);

        return items;
    }

    public async Task<IReadOnlyCollection<CatalogItemResponse>> ListPatientStatusesAsync(
        CancellationToken cancellationToken = default)
    {
        var items = await _context.PatientStatuses
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .Select(x => new CatalogItemResponse
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);

        return items;
    }

    public async Task<IReadOnlyCollection<CatalogItemResponse>> ListDocumentTypesAsync(
        CancellationToken cancellationToken = default)
    {
        var items = await _context.DocumentTypes
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .Select(x => new CatalogItemResponse
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);

        return items;
    }

    public async Task<IReadOnlyCollection<CatalogItemResponse>> ListConsentStatusesAsync(
        CancellationToken cancellationToken = default)
    {
        var items = await _context.ConsentStatuses
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .Select(x => new CatalogItemResponse
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);

        return items;
    }

    public async Task<IReadOnlyCollection<CatalogItemResponse>> ListTreatmentStatusesAsync(
        CancellationToken cancellationToken = default)
    {
        var items = await _context.TreatmentStatuses
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .Select(x => new CatalogItemResponse
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);

        return items;
    }

    public async Task<IReadOnlyCollection<CatalogItemResponse>> ListEvolutionStatusesAsync(
        CancellationToken cancellationToken = default)
    {
        var items = await _context.EvolutionStatuses
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .Select(x => new CatalogItemResponse
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);

        return items;
    }

    public async Task<Guid?> GetConsentStatusIdByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = string.IsNullOrWhiteSpace(code)
            ? string.Empty
            : code.Trim();

        return await _context.ConsentStatuses
            .AsNoTracking()
            .Where(x => x.IsActive && x.Code == normalizedCode)
            .Select(x => (Guid?)x.Id)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid?> GetTreatmentStatusIdByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = string.IsNullOrWhiteSpace(code)
            ? string.Empty
            : code.Trim();

        return await _context.TreatmentStatuses
            .AsNoTracking()
            .Where(x => x.IsActive && x.Code == normalizedCode)
            .Select(x => (Guid?)x.Id)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid?> GetEvolutionStatusIdByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = string.IsNullOrWhiteSpace(code)
            ? string.Empty
            : code.Trim();

        return await _context.EvolutionStatuses
            .AsNoTracking()
            .Where(x => x.IsActive && x.Code == normalizedCode)
            .Select(x => (Guid?)x.Id)
            .SingleOrDefaultAsync(cancellationToken);
    }
}
