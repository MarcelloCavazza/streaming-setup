using Microsoft.AspNetCore.Mvc;
using innerservice.Managers.Interfaces;
using Microsoft.AspNetCore.Http;
using Models.InnerService.Responses.Queues;
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

        private readonly ITasksManager _tasksManager;

        private readonly ILogger<QueuesController> _logger;

        public QueuesController(IQueuesManager queuesManager, ITasksManager tasksManager, ILogger<QueuesController> logger)
        {
            _queuesManager = queuesManager;
            _tasksManager = tasksManager;
            _logger = logger;
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveQueue([FromRoute] string id)
        {

            if (!Guid.TryParse(id, out var guid))
            {
                _logger.LogWarning("Invalid GUID input for removal: {Id}", id);
                return BadRequest("incorrent input");
            }

            var result = _tasksManager.TryCancelTask(guid);

            if (result)
            {
                var queueRemovalResult = _queuesManager.RemoveQueue(guid);
                if (queueRemovalResult)
                {
                    return Ok();
                }

                return Problem(statusCode: 500, detail: "Could not remove Queue");
            }

            return Problem(statusCode: 500, detail: "Could not cancel Task");
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetQueue([FromRoute] string id)
        {
            await Task.Delay(1000);

            if (!Guid.TryParse(id, out var guid))
            {
                return BadRequest("Invalid id provided");
            }

            var queue = _queuesManager.GetQueue(guid);

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

                    finalContent.Append(partialContent.Content);
                    fetchMore = partialContent.FetchMore;
                }
            }

            if (!fetchMore)
            {
                _queuesManager.RemoveQueue(guid);
            }

            return Ok(new QueueContent()
            {
                Content = finalContent.ToString(),
                FetchMore = fetchMore,
                QueueId = guid
            });
        }
    }
}