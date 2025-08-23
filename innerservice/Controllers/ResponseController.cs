using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace innerservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResponseController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetDataAsync()
        {
            var url = "http://127.0.0.1:5050/stream";

            var results = new List<string>();

            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            using var response = await httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                CancellationToken.None
            );

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new System.IO.StreamReader(stream);

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    Console.WriteLine(line);
                    results.Add(line);
                }
            }

            return Ok(results);
        }

        
    }
}