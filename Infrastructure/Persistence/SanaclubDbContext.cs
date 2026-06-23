using Microsoft.EntityFrameworkCore;
using Sanaclub.Application.Common.Abstractions;

namespace Sanaclub.Infrastructure.Persistence;

public sealed class SanaclubDbContext : DbContext, ISanaclubDbContext
{
    public SanaclubDbContext(DbContextOptions<SanaclubDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SanaclubDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
