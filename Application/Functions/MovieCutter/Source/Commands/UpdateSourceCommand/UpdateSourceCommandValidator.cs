using Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Functions.MovieCutter.Source.Commands.UpdateSourceCommand
{
    public class UpdateSourceCommandValidator : AbstractValidator<UpdateSourceCommand>
    {
        private readonly IMovieCutterDatabase _context;

        public UpdateSourceCommandValidator(IMovieCutterDatabase context)
        {
            _context = context;

            RuleFor(p => p.Name)
                .NotNull()
                .WithMessage("Name is required.")
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(100)
                .WithMessage("Name length can't be longer then 100 characters.");

            RuleFor(p => p.BaseUrl)
                .NotNull()
                .WithMessage("BaseUrl is required.")
                .NotEmpty()
                .WithMessage("BaseUrl is required.")
                .MaximumLength(200)
                .WithMessage("BaseUrl length can't be longer then 200 characters.");

            RuleFor(p => p.SourceType)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Value can't be less than 0.");

            RuleFor(p => p.IdProfile)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Value can't be less than 0.");

            RuleFor(p => p.IdSource)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Value can't be less than 0.");

            RuleFor(p => p)
                   .MustAsync(DoesSourceExists)
                   .WithMessage("Source not found");
        }

        private async Task<bool> DoesSourceExists(UpdateSourceCommand request, CancellationToken cancellationToken)
        {
            var doesSourceExists = await _context.Sources
                                                 .AnyAsync(p => p.IdSource == request.IdSource);

            return doesSourceExists;
        }
    }
}
