using Sanaclub.Infrastructure.Persistence.Seeders;
using Sanaclub.Api.Extensions;
using Sanaclub.Application;
using Sanaclub.Infrastructure;
using Serilog;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.AddApiLogging();

builder.Services.AddControllers();

const string SeedDatabaseArgument = "--seed-database";
const string CreateDevAdminArgument = "--create-dev-admin";
var shouldSeedDatabase = Array.Exists(args, arg =>
    string.Equals(arg, SeedDatabaseArgument, StringComparison.OrdinalIgnoreCase));
var shouldCreateDevAdmin = Array.Exists(args, arg =>
    string.Equals(arg, CreateDevAdminArgument, StringComparison.OrdinalIgnoreCase));
var isMaintenanceCommand = shouldSeedDatabase || shouldCreateDevAdmin;

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
if (!isMaintenanceCommand)
{
    builder.Services.AddApiAuthentication(builder.Configuration);
}

const string FrontendDevelopmentCorsPolicy = "FrontendDevelopment";
var frontendDevelopmentOrigins = new[]
{
    "http://localhost:5173",
    "https://localhost:5173",
    "http://127.0.0.1:5173",
    "https://127.0.0.1:5173"
};

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendDevelopmentCorsPolicy, policy =>
    {
        policy
            .WithOrigins(frontendDevelopmentOrigins)
            .AllowAnyHeader()
            .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
            .AllowCredentials();
    });
});

builder.Services.AddApiSwagger();

var app = builder.Build();

try
{
    if (shouldSeedDatabase && shouldCreateDevAdmin)
    {
        Log.Error("Only one maintenance command can be executed at a time.");
        Environment.ExitCode = 1;
        return;
    }

    if (shouldSeedDatabase)
    {
        if (!app.Environment.IsDevelopment())
        {
            Log.Error("Database seeding is only allowed in Development environment.");
            Environment.ExitCode = 1;
            return;
        }

        Log.Information("Starting database seeding.");

        using var scope = app.Services.CreateScope();
        var databaseSeeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
        await databaseSeeder.SeedAsync();

        Log.Information("Database seeding completed.");
        return;
    }

    if (shouldCreateDevAdmin)
    {
        if (!app.Environment.IsDevelopment())
        {
            Log.Error("Maintenance commands are only allowed in Development environment.");
            Environment.ExitCode = 1;
            return;
        }

        Log.Information("Starting development admin maintenance command.");

        using var scope = app.Services.CreateScope();
        var developmentAdminSeeder = scope.ServiceProvider.GetRequiredService<DevelopmentAdminSeeder>();
        await developmentAdminSeeder.SeedAsync();

        Log.Information("Development admin maintenance command completed.");
        return;
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors(FrontendDevelopmentCorsPolicy);
    }

    app.UseApiRequestLogging();

    app.UseApiExceptionHandling();

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
