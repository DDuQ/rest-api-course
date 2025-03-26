using FluentValidation;
using Movies.Application.Models;

namespace Movies.Application.Validators;

public class GetAllMovieOptionsValidator : AbstractValidator<GetAllMoviesOptions>
{
    public GetAllMovieOptionsValidator()
    {
        RuleFor(m => m.YearOfRelease)
            .LessThanOrEqualTo(DateTime.UtcNow.Year);
    }
}