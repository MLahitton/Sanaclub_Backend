using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Security;
using Sanaclub.Infrastructure.Documents;
using Sanaclub.Infrastructure.Security;
using Sanaclub.Infrastructure.Persistence.Repositories;
using Sanaclub.Infrastructure.Persistence.Seeders;
using Sanaclub.Infrastructure.Persistence;
using System;

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

        services.Configure<DocumentStorageOptions>(options =>
        {
            var section = configuration.GetSection(DocumentStorageOptions.SectionName);
            var configuredProvider = section["Provider"];
            if (!string.IsNullOrWhiteSpace(configuredProvider))
            {
                options.Provider = configuredProvider;
            }

            options.Local = new LocalDocumentStorageOptions
            {
                BasePath = section["Local:BasePath"]
            };

            if (string.IsNullOrWhiteSpace(options.Provider))
            {
                options.Provider = "Local";
            }

            if (options.Local is null)
            {
                options.Local = new LocalDocumentStorageOptions();
            }

            if (string.IsNullOrWhiteSpace(options.Local.BasePath))
            {
                options.Local.BasePath = LocalDocumentStorageOptions.GetDefaultBasePath();
            }
        });

        var storageProvider = configuration[$"{DocumentStorageOptions.SectionName}:Provider"];
        if (string.IsNullOrWhiteSpace(storageProvider))
        {
            storageProvider = "Local";
        }

        if (!storageProvider.Equals("Local", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"DocumentStorage provider '{storageProvider}' no está soportado en esta fase.");
        }

        services.AddScoped<IDocumentStorage, LocalDocumentStorage>();

        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<ICatalogRepository, CatalogRepository>();
        services.AddScoped<IConsentRepository, ConsentRepository>();
        services.AddScoped<ITreatmentSheetRepository, TreatmentSheetRepository>();
        services.AddScoped<IEvolutionSheetRepository, EvolutionSheetRepository>();
        services.AddScoped<IGeneratedDocumentRepository, GeneratedDocumentRepository>();
        services.AddScoped<ITreatmentSheetPdfGenerator, TreatmentSheetPdfGenerator>();
        services.AddScoped<IEvolutionSheetPdfGenerator, EvolutionSheetPdfGenerator>();


        services.AddScoped<AuthSeeder>();
        services.AddScoped<CatalogSeeder>();
        services.AddScoped<DevelopmentAdminSeeder>();
        services.AddScoped<DatabaseSeeder>();

        services.AddScoped<IDatabaseSeeder>(serviceProvider =>
            serviceProvider.GetRequiredService<DatabaseSeeder>());

        return services;
    }
}
