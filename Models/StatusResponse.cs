using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Models;

public class StatusResponse
{
    
    [JsonPropertyName("status")]
    public string Status { get; set; }
}