using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Extensions;

namespace Models
{
    public class FinalResponse
    {
        [JsonPropertyName("content")]
        public required string Content { get; set; }

        [JsonPropertyName("fetch_more")]
        public bool FetchMore { get; set; }

        [JsonPropertyName("queue_id"), JsonConverter(typeof(GuidStringConverter))]
        public Guid QueueId { get; set; }
    }
}