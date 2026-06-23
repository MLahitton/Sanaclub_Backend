using System.Threading;
using System.Threading.Tasks;

namespace Sanaclub.Infrastructure.Persistence.Seeders;

public sealed class DatabaseSeeder : IDatabaseSeeder
{
    private readonly AuthSeeder _authSeeder;
    private readonly CatalogSeeder _catalogSeeder;

    public DatabaseSeeder(AuthSeeder authSeeder, CatalogSeeder catalogSeeder)
    {
        _authSeeder = authSeeder;
        _catalogSeeder = catalogSeeder;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await _authSeeder.SeedAsync(cancellationToken);
        await _catalogSeeder.SeedAsync(cancellationToken);
    }
}

