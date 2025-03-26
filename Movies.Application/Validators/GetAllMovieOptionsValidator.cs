using FluentValidation;
using Movies.Application.Models;

namespace Movies.Application.Validators;

public class GetAllMovieOptionsValidator : AbstractValidator<GetAllMoviesOptions>
{
    
    private static readonly string[] AcceptableSortFields = ["title", "yearofrelease"];
    
    public GetAllMovieOptionsValidator()
    {
        RuleFor(m => m.SortField)
            .Must(sortField => sortField is null 
                               || AcceptableSortFields.Contains(sortField, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Sort field must be 'title' or 'yearofrelease'");
        
        RuleFor(m => m.YearOfRelease)
            .LessThanOrEqualTo(DateTime.UtcNow.Year);
    }
}