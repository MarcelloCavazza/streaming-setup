using System.Text.Json.Serialization;

namespace Models
{
    public class TrigerredResult
    {
        [JsonPropertyName("id")]
        public string Id {get; set;}
    }
}