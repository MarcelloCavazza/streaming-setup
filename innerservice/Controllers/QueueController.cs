using Microsoft.AspNetCore.Mvc;
using innerservice.Managers.Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

namespace innerservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QueueController : ControllerBase
    {
        private readonly IQueuesManager _queuesManager;

        private readonly ILogger<QueueController> _logger;

        public QueueController(IQueuesManager queuesManager, ILogger<QueueController> logger)
        {
            _queuesManager = queuesManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetQueue([FromQuery] string id)
        {
            await Task.Delay(1000);

            var queue = _queuesManager.GetQueue(id);

            _logger.LogTrace("Queue found {Object}", queue);

            if (queue == null)
            {
                return NotFound("Queue not found.");
            }

            // Return all values from the queue
            var finalContent = new StringBuilder();
            var fetchMore = false;

            lock (queue)
            {
                while (queue.Count != 0)
                {
                    var partialContent = queue.Dequeue();

                    finalContent.Append(partialContent.PartialContent);
                    fetchMore = partialContent.FetchMore;
                }
            }

            if (!fetchMore)
            {
                _queuesManager.RemoveQueue(id);
            }

            return Ok(new FinalResponse()
            {
                Content = finalContent.ToString(),
                FetchMore = fetchMore,
                QueueId = id
            });
        }
    }
}