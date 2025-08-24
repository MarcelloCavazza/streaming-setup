using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Models.InnerService.Enums;

namespace Models.InnerService.Responses.Tasks;

public class TaskStatusResponse
{
    [JsonPropertyName("status")]
    public TaskStatusEnum Status { get; set; }
}