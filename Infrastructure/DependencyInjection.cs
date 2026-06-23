using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Infrastructure.Persistence.Seeders;
using Sanaclub.Infrastructure.Persistence;

namespace Sanaclub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SanaclubDatabase");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "La cadena de conexión 'SanaclubDatabase' no está configurada.");
        }

        services.AddDbContext<SanaclubDbContext>(options =>
        {
            options.UseNpgsql(
                connectionString,
                npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(SanaclubDbContext).Assembly.FullName);
                });
        });

        services.AddScoped<ISanaclubDbContext>(serviceProvider =>
            serviceProvider.GetRequiredService<SanaclubDbContext>());

        services.AddScoped<AuthSeeder>();
        services.AddScoped<CatalogSeeder>();
        services.AddScoped<DatabaseSeeder>();

        services.AddScoped<IDatabaseSeeder>(serviceProvider =>
            serviceProvider.GetRequiredService<DatabaseSeeder>());

        return services;
    }
}
