using Movies.Application.Models;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.API.Mapping;

public static class ContractMapping
{
    public static Movie MapToMovie(this CreateMovieRequest source)
    {
        return new Movie
        {
            Title = source.Title,
            Genres = source.Genres.ToList(),
            Id = Guid.NewGuid(),
            YearOfRelease = source.YearOfRelease
        };
    }

    public static MovieResponse MapToResponse(this Movie source)
    {
        return new MovieResponse
        {
            Title = source.Title,
            Genres = source.Genres,
            Slug = source.Slug,
            Rating = source.Rating,
            UserRating = source.UserRating,
            Id = Guid.NewGuid(),
            YearOfRelease = source.YearOfRelease
        };
    }
    
    public static MoviesResponse MapToResponse(this IEnumerable<Movie> source, int page, int pageSize, int totalCount)
    {
        return new MoviesResponse()
        {
            Items = source.Select(MapToResponse),
            Page = page,
            PageSize = pageSize,
            Total = totalCount
        };
    }
    
    public static IEnumerable<MovieRatingResponse> MapToResponse(this IEnumerable<MovieRating> source)
    {
        return source.Select(movieRating => new MovieRatingResponse
        {
            Rating = movieRating.Rating,
            MovieId = movieRating.MovieId,
            Slug = movieRating.Slug
        });
    }
    
    public static Movie MapToMovie(this UpdateMovieRequest source, Guid id)
    {
        return new Movie
        {
            Id = id,
            Title = source.Title,
            Genres = source.Genres.ToList(),
            YearOfRelease = source.YearOfRelease
        };
    }

    public static GetAllMoviesOptions MapToOptions(this GetAllMoviesRequest source)
    {
        return new GetAllMoviesOptions()
        {
            Title = source.Title,
            YearOfRelease = source.YearOfRelease,
            SortField = source.SortBy?.Trim('+', '-'),
            SortOrder = source.SortBy is null ? SortOrder.Unsorted : 
                source.SortBy.StartsWith('+')  ? SortOrder.Ascending : SortOrder.Descending,
            Page = source.Page,
            PageSize = source.PageSize
        };
    }

    public static GetAllMoviesOptions WithUser(this GetAllMoviesOptions source, Guid? userId)
    {
        source.UserId = userId;
        return source;
    }
}