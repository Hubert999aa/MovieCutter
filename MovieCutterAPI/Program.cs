using Application;
using Application.Functions.Maintenance.ApplyDatabaseMigrationsCommand;
using Domain.TechnicalModels;
using MassTransit;
using MovieCutterAPI.Hubs;
using MyMediator.Interfaces;
using Persistance;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration);
});

builder.Services.Configure<FolderPathsOptions>(builder.Configuration.GetSection("FolderPaths"));

builder.Services.AddApplicationLayer(builder.Configuration);
builder.Services.AddPersistanceLayer(builder.Configuration);

var rabbitMqSettings = builder.Configuration.GetSection("RabbitMQ")
                .Get<RabbitMQBaseSettings>()
                ?? throw new InvalidOperationException("RabbitMQ configuration is missing");

builder.Services.AddSignalR();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqSettings.Host, rabbitMqSettings.VirtualHost, h =>
        {
            h.Username(rabbitMqSettings.Username);
            h.Password(rabbitMqSettings.Password);
        });
    });
});

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("OpenCors", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("OpenCors");

app.MapControllers();
app.MapHub<OperationsProgressHub>("/hubs/operationsProgress");

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    var mediator = services.GetRequiredService<IMediator>();

    await mediator.Send(new ApplyDatabaseMigrationsCommand());
}

app.Run();