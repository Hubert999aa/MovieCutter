using Application;
using Application.Functions.Maintenance.ApplyDatabaseMigrationsCommand;
using MyMediator.Interfaces;
using Domain.TechnicalModels;
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

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    var mediator = services.GetRequiredService<IMediator>();

    await mediator.Send(new ApplyDatabaseMigrationsCommand());
}

app.Run();

//ToDo:
// 1. Add UI for the application


//Ideas:
// 1. Extend download feature, so we will be able to cut the video into frames straight after downloading
// 2. Add possibility to check current progress of the process
// 3. Create tests in the application