using Microsoft.AspNetCore.Mvc;
using innerservice.Managers.Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace innerservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QueueController : ControllerBase
    {
        private readonly IQueuesManager _queuesManager;

        public QueueController(IQueuesManager queuesManager)
        {
            _queuesManager = queuesManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetQueue([FromQuery] string id)
        {
            await Task.Delay(1000); // 1 second delay at the beginning

            // Try to get the queue from the manager
            var queue = _queuesManager.GetQueue(id);

            if (queue == null)
            {
                return NotFound("Queue not found.");
            }

            // Return all values from the queue
            var values = new List<PartialContentResponse>();
            lock (queue)
            {
                values.AddRange(queue);
            }

            return Ok(values);
        }
    }
}