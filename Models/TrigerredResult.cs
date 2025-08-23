using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Models
{
    public class TrigerredResult
    {
        [JsonPropertyName("id")]
        public string Id {get; set;}
    }
}