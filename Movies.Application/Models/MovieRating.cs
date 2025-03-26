namespace Movies.Application.Models;

public class MovieRating
{
    public required Guid MovieId { get; set; }
    public required float Slug { get; set; }
    public required int Rating { get; set; }
}