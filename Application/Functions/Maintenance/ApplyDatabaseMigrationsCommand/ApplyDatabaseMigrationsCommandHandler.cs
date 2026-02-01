using Application.Interfaces;
using Application.Mediator;
using Domain.TechnicalModels;
using Serilog;

namespace Application.Functions.Maintenance.ApplyDatabaseMigrationsCommand
{
    public class ApplyDatabaseMigrationsCommandHandler(IMovieCutterDatabase _context) : IRequestHandler<ApplyDatabaseMigrationsCommand, BaseResponse>
    {
        public async Task<BaseResponse> Handle(ApplyDatabaseMigrationsCommand request, CancellationToken cancellationToken)
        {
            Log.Information("Starting calorie database migrations update checking");
            await _context.MigrateAsync();

            return new BaseResponse();
        }
    }
}
