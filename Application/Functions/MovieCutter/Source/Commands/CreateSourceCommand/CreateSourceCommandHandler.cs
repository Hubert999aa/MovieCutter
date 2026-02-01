using Application.Interfaces;
using Application.Mediator;
using Domain.BusinessEnums;
using Domain.TechnicalModels;

namespace Application.Functions.MovieCutter.Source.Commands.CreateSourceCommand
{
    public class CreateSourceCommandHandler(IMovieCutterDatabase _context) : IRequestHandler<CreateSourceCommand, BaseResponse>
    {
        public async Task<BaseResponse> Handle(CreateSourceCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateSourceCommandValidator();
            var validatorResult = await validator.ValidateAsync(request);
            if (!validatorResult.IsValid) return new BaseResponse(validatorResult);

            var newSource = new Domain.BusinessModels.DatabaseModels.Source
            {
                Name = request.Name,
                BaseUrl = request.BaseUrl,
                SourceType = (SourceType)request.SourceType,
                IdProfile = request.IdProfile,
            };

            await _context.Sources.AddAsync(newSource);
            await _context.SaveChangesAsync();

            return new BaseResponse();
        }
    }
}
