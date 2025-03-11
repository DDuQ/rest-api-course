using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly List<Movie> _movies = [];
    
    public Task<bool> CreateAsync(Movie movie)
    {
        _movies.Add(movie);
        return Task.FromResult(true);
    }

    public Task<Movie?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_movies.FirstOrDefault(x => x.Id == id));
    }

    public Task<IEnumerable<Movie>> GetAllAsync()
    {
        return Task.FromResult(_movies.AsEnumerable());
    }

    public Task<bool> UpdateAsync(Movie movie)
    {
        var movieIndex = _movies.FindIndex(x => x.Id == movie.Id);
        if (movieIndex == -1)
        {
            return Task.FromResult(false);
        }

        _movies[movieIndex] = movie;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var removeCount = _movies.RemoveAll(x => x.Id.Equals(id));
        return Task.FromResult(removeCount > 0);
    }
}