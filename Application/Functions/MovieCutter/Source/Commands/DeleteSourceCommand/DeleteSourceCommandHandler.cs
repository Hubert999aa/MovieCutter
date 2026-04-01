using Application.Interfaces;
using MyMediator.Interfaces;
using Domain.TechnicalModels;
using Microsoft.EntityFrameworkCore;

namespace Application.Functions.MovieCutter.Source.Commands.DeleteSourceCommand
{
    public class DeleteSourceCommandHandler(IMovieCutterDatabase _context) : IRequestHandler<DeleteSourceCommand, BaseResponse>
    {
        public async Task<BaseResponse> Handle(DeleteSourceCommand request, CancellationToken cancellationToken)
        {
            var source = await _context.Sources
                .SingleOrDefaultAsync(p => p.IdSource == request.IdSource);

            if (source != null)
            {
                _context.Sources.Remove(source);
                await _context.SaveChangesAsync();
            }

            return new BaseResponse();
        }
    }
}
