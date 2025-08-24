using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using innerservice.Managers;
using Models;
using Microsoft.Extensions.Logging;
using innerservice.Managers.Interfaces;

namespace innerservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITasksManager _tasksManager;
        private readonly IQueuesManager _queuesManager;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITasksManager tasksManager, ILogger<TasksController> logger, IQueuesManager queuesManager)
        {
            _tasksManager = tasksManager;
            _queuesManager = queuesManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAllTasks()
        {
            var tasks = _tasksManager.GetAllTasks();
            _logger.LogInformation("Retrieved all tasks.");
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public IActionResult GetStatus([FromRoute] string id)
        {
            if (!Guid.TryParse(id, out var guid))
            {
                _logger.LogWarning("Invalid GUID input for status: {Id}", id);
                return BadRequest("incorrent input");
            }

            _tasksManager.TryGetStatus(guid, out var status);
            _logger.LogInformation("Status retrieved for task: {Id}", id);
            return Ok(new StatusResponse()
            {
                Status = status
            });
        }

        [HttpDelete("{id}")]
        public IActionResult CancelTask([FromRoute] string id)
        {
            if (!Guid.TryParse(id, out var guid))
            {
                _logger.LogWarning("Invalid GUID input for cancel: {Id}", id);
                return BadRequest("incorrent input");
            }

            var result = _tasksManager.TryCancelTask(guid);
            if (!result)
            {
                _logger.LogWarning("Cancel failed for task: {Id}", id);
                return NotFound();
            }

            _queuesManager.RemoveQueue(guid);

            _logger.LogInformation("Task cancelled: {Id}", id);
            return Ok();
        }
    }
}