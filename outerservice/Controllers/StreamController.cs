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
    private readonly Random _random = new Random();

    public StreamController(ILogger<StreamController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task StreamAsync()
    {
        Response.ContentType = "application/x-ndjson";

        int position = 0;
        int totalLength = Constants.MOCK_RESPONSE.Length;

        while (position < totalLength)
        {
            int chunkSize = _random.Next(1, 6);
            if (position + chunkSize > totalLength)
                chunkSize = totalLength - position;

            string chunk = Constants.MOCK_RESPONSE.Substring(position, chunkSize);

            _logger.LogInformation("Sending chunk: {Chunk}", chunk);

            var json = JsonSerializer.Serialize(new Response()
            {
                Id = Guid.NewGuid(),
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
