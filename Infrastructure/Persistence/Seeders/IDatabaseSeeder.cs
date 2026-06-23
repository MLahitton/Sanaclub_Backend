using System.Threading;
using System.Threading.Tasks;

namespace Sanaclub.Infrastructure.Persistence.Seeders;

public interface IDatabaseSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}

