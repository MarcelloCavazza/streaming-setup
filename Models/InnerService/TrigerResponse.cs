using System.Text.Json.Serialization;

namespace Models.InnerService;
public class TriggerResponse
{
    [JsonPropertyName("id")]
    public string Id {get; set;}
}