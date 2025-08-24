using System.Text.Json.Serialization;
using Models.Extensions;

namespace Models.InnerService;
public class TriggerResponse
{
    [JsonPropertyName("id"), JsonConverter(typeof(GuidStringConverter))]
    public Guid Id {get; set;}
}