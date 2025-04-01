using System.Text.Json.Serialization;

namespace Movies.Contracts.Responses;

public class HalResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<Link> Links { get; set; } = [];
}

public class Link
{
    public required string Href { get; init; }
    public required string Rel { get; init; }
    public required string Type { get; init; }
}