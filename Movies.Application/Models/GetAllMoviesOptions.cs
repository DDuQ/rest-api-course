using Movies.Contracts.Requests;

namespace Movies.Application.Models;

public class GetAllMoviesOptions : PagedRequest
{
    public string? Title { get; set; }
    public int? YearOfRelease { get; set; }
    public Guid? UserId { get; set; }
    public string? SortField { get; set; }
    public SortOrder? SortOrder { get; set; }
}

public enum SortOrder
{
    Unsorted,
    Ascending,
    Descending
}