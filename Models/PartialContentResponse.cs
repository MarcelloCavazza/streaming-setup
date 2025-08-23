using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Models
{
    public class PartialContentResponse
    {
        [JsonPropertyName("queueGuid")]
        public string QueueGUID { get; set; }
        
        [JsonPropertyName("partialContent")]
        public string PartialContent { get; set; }

        [JsonPropertyName("fetchMore")]
        public bool FetchMore { get; set; }
    }
}