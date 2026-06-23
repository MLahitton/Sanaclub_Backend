using Sanaclub.Infrastructure.Persistence.Seeders;
using Sanaclub.Api.Extensions;
using Sanaclub.Application;
using Sanaclub.Infrastructure;
using Serilog;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.AddApiLogging();

builder.Services.AddControllers();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

const string SeedDatabaseArgument = "--seed-database";
var shouldSeedDatabase = Array.Exists(args, arg =>
    string.Equals(arg, SeedDatabaseArgument, StringComparison.OrdinalIgnoreCase));

var app = builder.Build();

try
{
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

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseApiRequestLogging();

    app.UseApiExceptionHandling();

    app.UseHttpsRedirection();

    app.MapControllers();

    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
