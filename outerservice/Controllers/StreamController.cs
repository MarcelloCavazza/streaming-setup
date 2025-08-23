using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Models;

using outerservice.Extensions;

namespace outerservice.Controllers;

[ApiController]
[Route("[controller]")]
public class StreamController : ControllerBase
{

    [HttpPost]
    public async Task StreamAsync()
    {
        Response.ContentType = "application/x-ndjson";

        for (int i = 1; i <= 1000; i++)
        {
            var json = JsonSerializer.Serialize(new Response()
            {
                Id = Guid.NewGuid().ToString(),
                Content = RandomStringGenerator.Generate()
            }) + "\n"; // NDJSON: newline after each object

            var bytes = Encoding.UTF8.GetBytes(json);

            await Response.Body.WriteAsync(bytes, 0, bytes.Length);
            await Response.Body.FlushAsync();

            await Task.Delay(200);
        }

    }

}
