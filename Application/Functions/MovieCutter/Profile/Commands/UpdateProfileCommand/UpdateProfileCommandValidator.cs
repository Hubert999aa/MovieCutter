using Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Functions.MovieCutter.Profile.Commands.UpdateProfileCommand
{
    public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
    {
        private readonly IMovieCutterDatabase _context;

        public UpdateProfileCommandValidator(IMovieCutterDatabase context)
        {
            _context = context;

            RuleFor(p => p.Name)
                .NotNull()
                .WithMessage("Name is required.")
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(100)
                .WithMessage("Name length can't be longer then 100 characters.");

            RuleFor(p => p)
                   .MustAsync(DoesProfileExists)
                   .WithMessage("Profile not found");
        }

        private async Task<bool> DoesProfileExists(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var doesProfileExists = await _context.Profiles
                                                  .AnyAsync(p => p.IdProfile == request.IdProfile);

            return doesProfileExists;
        }
    }
}
