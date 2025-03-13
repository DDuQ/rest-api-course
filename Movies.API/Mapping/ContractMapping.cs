using Movies.Application.Models;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.API.Mapping;

public static class ContractMapping
{
    public static Movie MapToMovie(this CreateMovieRequest source)
    {
        return new Movie()
        {
            Title = source.Title,
            Genres = source.Genres.ToList(),
            Id = Guid.NewGuid(),
            YearOfRelease = source.YearOfRelease
        };
    }

    public static MovieResponse MapToResponse(this Movie source)
    {
        return new MovieResponse()
        {
            Title = source.Title,
            Genres = source.Genres,
            Slug = source.Slug,
            Id = Guid.NewGuid(),
            YearOfRelease = source.YearOfRelease
        };
    }

    public static MoviesResponse MapToResponse(this IEnumerable<Movie> source)
    {
        return new MoviesResponse()
        {
            Items = source.Select(MapToResponse)
        };
    }

    public static Movie MapToMovie(this UpdateMovieRequest source, Guid id)
    {
        return new Movie()
        {
            Id = id,
            Title = source.Title,
            Genres = source.Genres.ToList(),
            YearOfRelease = source.YearOfRelease
        };
    }
}