using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using Models;

using outerservice.Extensions;

namespace outerservice.Controllers;

[ApiController]
[Route("[controller]")]
public class StreamController : ControllerBase
{
    private readonly ILogger<StreamController> _logger;

    public StreamController(ILogger<StreamController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task StreamAsync()
    {
        Response.ContentType = "application/x-ndjson";

        string bigText = RandomStringGenerator.MOCK_RESPONSE;
        int position = 0;
        int totalLength = bigText.Length;

        while (position < totalLength)
        {
            int chunkSize = new Random().Next(1, 6);
            if (position + chunkSize > totalLength)
                chunkSize = totalLength - position;

            string chunk = bigText.Substring(position, chunkSize);

            _logger.LogInformation("Sending chunk: {Chunk}", chunk);

            var json = JsonSerializer.Serialize(new Response()
            {
                Id = Guid.NewGuid().ToString(),
                Content = chunk,
                FetchMore = position + chunkSize < totalLength
            }) + "\n";

            var bytes = Encoding.UTF8.GetBytes(json);

            await Response.Body.WriteAsync(bytes);
            await Response.Body.FlushAsync();

            position += chunkSize;
            await Task.Delay(100);
        }

    }

}
