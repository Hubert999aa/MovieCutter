using Application.Interfaces;
using CoreModels.TechnicalModels;
using Domain.ResponseModels;
using Microsoft.EntityFrameworkCore;
using MyMediator.Interfaces;

namespace Application.Functions.MovieCutter.Operation.Queries.GetOperationStatusesQuery
{
    public class GetOperationStatusesQueryHandler(IMovieCutterDatabase _context) : IRequestHandler<GetOperationStatusesQuery, BaseResponse>
    {
        public async Task<BaseResponse> Handle(GetOperationStatusesQuery request, CancellationToken cancellationToken)
        {
            var lastFiveMinutes = DateTime.UtcNow.AddMinutes(-5);
            var operations = await _context.Operations
                .Where(p => p.CreationnDate >= lastFiveMinutes)
                .Select(p => new BaseOperationState
                {
                    IdOperation = p.IdOperation,
                    OperationStatus = p.OperationStatus.ToString(),
                    OperationType = p.OperationType.ToString(),
                    VideoName = p.VideoName,
                    ProgressPercentage = p.ProgressPercentage,

                })
                .ToListAsync();

            return new BaseResponse(operations);
        }
    }
}
