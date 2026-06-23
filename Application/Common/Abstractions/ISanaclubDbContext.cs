namespace Sanaclub.Application.Common.Abstractions;

public interface ISanaclubDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
