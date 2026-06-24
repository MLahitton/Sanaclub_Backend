using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Security;
using Sanaclub.Infrastructure.Security;
using Sanaclub.Infrastructure.Persistence.Repositories;
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
                "The connection string 'SanaclubDatabase' is not configured.");
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

        var jwtSection = configuration.GetSection(JwtOptions.SectionName);

        services.AddOptions<JwtOptions>()
            .Configure(options =>
            {
                options.Issuer = jwtSection["Issuer"] ?? string.Empty;
                options.Audience = jwtSection["Audience"] ?? string.Empty;
                options.Secret = jwtSection["Secret"] ?? string.Empty;

                if (int.TryParse(jwtSection["AccessTokenExpirationMinutes"], out var accessTokenExpirationMinutes)
                    && accessTokenExpirationMinutes > 0)
                {
                    options.AccessTokenExpirationMinutes = accessTokenExpirationMinutes;
                }

                if (int.TryParse(jwtSection["RefreshTokenExpirationDays"], out var refreshTokenExpirationDays)
                    && refreshTokenExpirationDays > 0)
                {
                    options.RefreshTokenExpirationDays = refreshTokenExpirationDays;
                }
            });

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IAuthRepository, AuthRepository>();

        services.AddScoped<AuthSeeder>();
        services.AddScoped<CatalogSeeder>();
        services.AddScoped<DevelopmentAdminSeeder>();
        services.AddScoped<DatabaseSeeder>();

        services.AddScoped<IDatabaseSeeder>(serviceProvider =>
            serviceProvider.GetRequiredService<DatabaseSeeder>());

        return services;
    }
}
