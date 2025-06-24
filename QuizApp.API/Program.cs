using QuizApp.API.Extensions;
using QuizApp.Application.Common.Extensions;
using QuizApp.Infrastructure.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    var services = builder.Services;
    var configuration = builder.Configuration;

    // Add layer services
    services.AddInfrastructureServices(configuration);
    services.AddApplicationServices();
    services.AddApiServices();

    var app = builder.Build();

    // Configure the HTTP request pipeline
    app.ConfigurePipeline();

    // Database Migration and Seeding
    await app.MigrateDatabaseAsync();

    Log.Information("Starting QuizApp API");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}