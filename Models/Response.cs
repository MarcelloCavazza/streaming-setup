using System.Text.Json.Serialization;

namespace Models;

public class Response
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("id")]
    public required Guid Id { get; set; }

    [JsonPropertyName("fetch_more")]
    public required bool FetchMore { get; set; }
}
