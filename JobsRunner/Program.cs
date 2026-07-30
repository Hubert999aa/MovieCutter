using Application;
using JobsRunner;
using JobsRunner.Interfaces;
using JobsRunner.Options;
using JobsRunner.Services;
using Persistance;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

// Serilog
builder.Services.AddSerilog(config =>
    config.ReadFrom.Configuration(builder.Configuration));

// RabbitMQ
var rabbitMqSettings = builder.Configuration
    .GetSection("RabbitMQ")
    .Get<RabbitMQExtendedSettings>()
    ?? throw new InvalidOperationException("RabbitMQ configuration is missing");

// MassTransit
JobsRunnerStartupConfiguration.ConfigureMassTransit(builder.Services, builder.Configuration, rabbitMqSettings);

// Persistance and Application
builder.Services.AddApplicationLayer(builder.Configuration);
builder.Services.AddPersistanceLayer(builder.Configuration);

// JobRunner services
builder.Services.AddScoped<IVideoDownloader, VideoDownloaderService>();

var host = builder.Build();

try
{
    Log.Information("MovieCutterJobsRunner starting...");
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "MovieCutterJobsRunner terminated unexpectedly");
    throw;
}
finally
{
    await Log.CloseAndFlushAsync();
}
