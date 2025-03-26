using Movies.Application.Models;

namespace Movies.Application.Services;

public interface IMovieService
{
    Task<bool> CreateAsync(Movie movie, CancellationToken token = default);
    Task<Movie?> GetByIdAsync(Guid id, Guid? userId = null, CancellationToken token = default);
    Task<Movie?> GetBySlugAsync(string slug, Guid? userId = null, CancellationToken token = default);
    Task<IEnumerable<Movie>> GetAllAsync(Guid? userId = null, CancellationToken token = default);
    Task<Movie?> UpdateAsync(Movie movie, Guid? userId = null, CancellationToken token = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken token = default);
}