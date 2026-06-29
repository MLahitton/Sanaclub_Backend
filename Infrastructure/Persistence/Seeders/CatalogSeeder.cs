using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Sanaclub.Infrastructure.Persistence;
using Sanaclub.Infrastructure.Persistence.SeedData;

namespace Sanaclub.Infrastructure.Persistence.Seeders;

public sealed class CatalogSeeder : IDatabaseSeeder
{
    private readonly SanaclubDbContext _context;
    private readonly HashSet<string> _allowedTables = new()
    {
        "identification_types",
        "genders",
        "civil_statuses",
        "document_types",
        "patient_statuses",
        "consent_statuses",
        "treatment_statuses",
        "evolution_statuses",
        "appointment_statuses"
    };

    public CatalogSeeder(SanaclubDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedCatalogItemsAsync(
            "identification_types",
            CatalogSeedData.IdentificationTypes,
            cancellationToken);

        await SeedCatalogItemsAsync(
            "genders",
            CatalogSeedData.Genders,
            cancellationToken);

        await SeedCatalogItemsAsync(
            "civil_statuses",
            CatalogSeedData.CivilStatuses,
            cancellationToken);

        await SeedCatalogItemsAsync(
            "document_types",
            CatalogSeedData.DocumentTypes,
            cancellationToken);

        await SeedCatalogItemsAsync(
            "patient_statuses",
            CatalogSeedData.PatientStatuses,
            cancellationToken);

        await SeedCatalogItemsAsync(
            "consent_statuses",
            CatalogSeedData.ConsentStatuses,
            cancellationToken);

        await SeedCatalogItemsAsync(
            "treatment_statuses",
            CatalogSeedData.TreatmentStatuses,
            cancellationToken);

        await SeedCatalogItemsAsync(
            "evolution_statuses",
            CatalogSeedData.EvolutionStatuses,
            cancellationToken);

        await SeedCatalogItemsAsync(
            "appointment_statuses",
            CatalogSeedData.AppointmentStatuses,
            cancellationToken);
    }

    private async Task SeedCatalogItemsAsync(
        string tableName,
        IReadOnlyCollection<SeedCatalogItem> items,
        CancellationToken cancellationToken)
    {
        var safeTableName = GetAllowedCatalogTableNameOrThrow(tableName);

        var now = DateTime.UtcNow;

        foreach (var item in items)
        {
            var sql = $@"
                INSERT INTO catalog.{safeTableName} (
                    id,
                    code,
                    name,
                    description,
                    is_active,
                    sort_order,
                    created_at_utc,
                    created_by_user_id,
                    updated_at_utc,
                    updated_by_user_id)
                VALUES (
                    @id,
                    @code,
                    @name,
                    @description,
                    true,
                    @sortOrder,
                    @now,
                    NULL,
                    NULL,
                    NULL)
                ON CONFLICT (id) DO UPDATE SET
                    code = EXCLUDED.code,
                    name = EXCLUDED.name,
                    description = EXCLUDED.description,
                    is_active = true,
                    sort_order = EXCLUDED.sort_order,
                    updated_at_utc = @now,
                    updated_by_user_id = NULL;";

            var parameters = new object[]
            {
                new NpgsqlParameter("id", item.Id),
                new NpgsqlParameter("code", item.Code),
                new NpgsqlParameter("name", item.Name),
                new NpgsqlParameter("description", item.Description ?? (object)DBNull.Value),
                new NpgsqlParameter("sortOrder", item.SortOrder),
                new NpgsqlParameter("now", now)
            };

            await _context.Database.ExecuteSqlRawAsync(
                sql,
                parameters,
                cancellationToken);
        }
    }

    private string GetAllowedCatalogTableNameOrThrow(string tableName)
    {
        if (!_allowedTables.Contains(tableName))
        {
            throw new InvalidOperationException("Invalid catalog table name.");
        }

        return tableName;
    }
}
