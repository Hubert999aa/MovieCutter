using Application.Interfaces;
using MyMediator.Interfaces;
using Domain.BusinessEnums;
using CoreModels.TechnicalModels;
using Microsoft.EntityFrameworkCore;

namespace Application.Functions.MovieCutter.Source.Commands.UpdateSourceCommand
{
    public class UpdateSourceCommandHandler(IMovieCutterDatabase _context) : IRequestHandler<UpdateSourceCommand, BaseResponse>
    {
        public async Task<BaseResponse> Handle(UpdateSourceCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateSourceCommandValidator(_context);
            var validatorResult = await validator.ValidateAsync(request);
            if (!validatorResult.IsValid) return new BaseResponse(validatorResult);

            var source = await _context.Sources
                .SingleAsync(p => p.IdSource == request.IdSource);

            if (source.Name != request.Name) source.Name = request.Name;
            if (source.BaseUrl != request.BaseUrl) source.BaseUrl = request.BaseUrl;
            if (source.SourceType != (SourceType)request.SourceType) source.SourceType = (SourceType)request.SourceType;
            if (source.IdProfile != request.IdProfile) source.IdProfile = request.IdProfile;

            await _context.SaveChangesAsync();

            return new BaseResponse();
        }
    }
}
