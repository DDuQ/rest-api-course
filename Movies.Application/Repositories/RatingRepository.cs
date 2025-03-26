using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class RatingRepository : IRatingRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public RatingRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> RateMovieAsync(
        Guid movieId,
        int rating,
        Guid userId,
        CancellationToken token = default
    )
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var result = await connection.ExecuteAsync(
            new CommandDefinition(
                """
                INSERT INTO ratings (movieId, rating, userId)
                VALUES (@movieId, @rating, @userId)
                ON CONFLICT (userid, movieid) DO UPDATE 
                    SET rating = @rating
                """,
                new
                {
                    movieId,
                    rating,
                    userId,
                },
                cancellationToken: token
            )
        );

        return result > 0;
    }

    public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        return await connection.QueryFirstOrDefaultAsync(
            new CommandDefinition(
                """
                SELECT round(avg(r.rating), 1) from ratings r
                WHERE movieid = @movieId
                """,
                new { movieId },
                cancellationToken: token
            )
        );
    }

    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(
        Guid movieId,
        Guid userId,
        CancellationToken token = default
    )
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        return await connection.QueryFirstOrDefaultAsync(
            new CommandDefinition(
                """
                SELECT round(avg(r.rating), 1),
                       (SELECT rating
                        FROM ratings
                        WHERE movieid = @movieId 
                          AND userid = @userId
                        LIMIT 1)
                FROM ratings r
                WHERE movieid = @movieId
                """,
                new { movieId, userId },
                cancellationToken: token
            )
        );
    }

    public async Task<bool> DeleteAsync(
        Guid movieId,
        Guid userId,
        CancellationToken token = default
    )
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var result = await connection.ExecuteAsync(
            new CommandDefinition(
                """
                DELETE FROM ratings
                WHERE movieid = @movieId
                    AND userid = @userId
                """,
                new { movieId, userId },
                cancellationToken: token
            )
        );

        return result > 0;
    }

    public async Task<IEnumerable<MovieRating>> GetRatingsAsync(
        Guid userId,
        CancellationToken token = default
    )
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        return await connection.QueryAsync<MovieRating>(
            new CommandDefinition(
                """
                SELECT r.rating, r.movieid, m.slug 
                FROM ratings r
                INNER JOIN movies m on r.movieid = m.id
                WHERE userid = @userId
                """,
                new { userId },
                cancellationToken: token
            )
        );
    }
}
