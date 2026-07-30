using JobsRunner;
using JobsRunner.Options;
using Serilog;
using Persistance;
using Application;

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

//Persistance and Application
builder.Services.AddApplicationLayer(builder.Configuration);
builder.Services.AddPersistanceLayer(builder.Configuration);


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
