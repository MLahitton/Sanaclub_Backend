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
}
