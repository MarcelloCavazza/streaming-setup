using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Extensions;

namespace Models
{
    public class PartialContentResponse
    {
        [JsonPropertyName("queueGuid"), JsonConverter(typeof(GuidStringConverter))]
        public Guid QueueGUID { get; set; }
        
        [JsonPropertyName("partialContent")]
        public string PartialContent { get; set; }

        [JsonPropertyName("fetchMore")]
        public bool FetchMore { get; set; }
    }
}