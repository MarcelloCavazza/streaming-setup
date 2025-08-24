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
    public class QueuesController : ControllerBase
    {
        private readonly IQueuesManager _queuesManager;

        private readonly ILogger<QueuesController> _logger;

        public QueuesController(IQueuesManager queuesManager, ILogger<QueuesController> logger)
        {
            _queuesManager = queuesManager;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQueue([FromRoute] string id)
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