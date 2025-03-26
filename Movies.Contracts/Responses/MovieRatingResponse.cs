namespace Movies.Contracts.Responses;

public class MovieRatingResponse
{
    public required Guid MovieId { get; set; }
    public required float Slug { get; set; }
    public required int Rating { get; set; }
}