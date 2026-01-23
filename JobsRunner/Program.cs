using JobsRunner;
using JobsRunner.Options;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

// Serilog
builder.Services.AddSerilog(config =>
    config.ReadFrom.Configuration(builder.Configuration));

// RabbitMQ
var rabbitMqSettings = builder.Configuration
    .GetSection("RabbitMQ")
    .Get<RabbitMQSettings>()
    ?? throw new InvalidOperationException("RabbitMQ configuration is missing");

builder.Services.AddSingleton(rabbitMqSettings);

// MassTransit
MassTransitConfiguration.ConfigureMassTransit(builder.Services, builder.Configuration, rabbitMqSettings);



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
