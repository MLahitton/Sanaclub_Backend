using Sanaclub.Api.Extensions;
using Sanaclub.Application;
using Sanaclub.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddApiLogging();

builder.Services.AddControllers();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseApiRequestLogging();

app.UseApiExceptionHandling();

app.UseHttpsRedirection();

app.MapControllers();

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
