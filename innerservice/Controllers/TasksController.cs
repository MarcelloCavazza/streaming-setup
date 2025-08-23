using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using innerservice.Managers;
using Models;

namespace innerservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly TasksManager _tasksManager;

        public TasksController(TasksManager tasksManager)
        {
            _tasksManager = tasksManager;
        }

        [HttpGet]
        public IActionResult GetAllTasks()
        {
            var tasks = _tasksManager.GetAllTasks();
            return Ok(tasks);
        }

        [HttpGet("status")]
        public IActionResult GetStatus([FromQuery] string id)
        {
            if (!Guid.TryParse(id, out var guid))
            {
                return BadRequest("incorrent input");
            }

            _tasksManager.TryGetStatus(guid, out var status);
            return Ok(status);
        }

        [HttpPost("cancel")]
        public IActionResult CancelTask([FromBody] string id)
        {
            if (!Guid.TryParse(id, out var guid))
            {
                return BadRequest("incorrent input");
            }

            var result = _tasksManager.TryCancelTask(guid);
            if (!result)
                return NotFound();
            return Ok();
        }
    }
}