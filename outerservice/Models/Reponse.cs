using System.Text.Json.Serialization;

namespace outerservice.Models;

public class Response
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("id")]
    public required string Id { get; set; }
}