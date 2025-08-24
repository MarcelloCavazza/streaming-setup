using System.Collections.Concurrent;
using innerservice.Managers.Interfaces;
using Microsoft.Extensions.Logging;

namespace innerservice.Managers
{
    public class TasksManager : ITasksManager
    { 
        private readonly ConcurrentDictionary<Guid, (Task Task, CancellationTokenSource Cts)> _tasks = new();
        private readonly ILogger<TasksManager> _logger;

        public TasksManager(ILogger<TasksManager> logger)
        {
            _logger = logger;
        }

        public Guid StartTask(Func<CancellationToken, Guid, Task> work)
        {
            var cts = new CancellationTokenSource();
            var id = Guid.NewGuid();

            var task = Task.Run(() => work(cts.Token, id), cts.Token);

            _tasks[id] = (task, cts);

            _logger.LogInformation("Started task: {TaskId}", id);

            // Remove task from dictionary when completed
            _ = task.ContinueWith(_ => {
                _tasks.TryRemove(id, out var _);
                _logger.LogInformation("Task completed and removed: {TaskId}", id);
            });

            return id;
        }
        
        public bool TryCancelTask(Guid id)
        {
            if (_tasks.TryGetValue(id, out var entry))
            {
                entry.Cts.Cancel();
                _logger.LogInformation("Cancelled task: {TaskId}", id);
                return true;
            }
            _logger.LogWarning("Attempted to cancel non-existent task: {TaskId}", id);
            return false;
        }

        public bool TryGetStatus(Guid id, out string status)
        {
            if (_tasks.TryGetValue(id, out var entry))
            {
                if (entry.Task.IsCanceled) status = "Canceled";
                else if (entry.Task.IsCompletedSuccessfully) status = "Completed";
                else if (entry.Task.IsFaulted) status = "Faulted";
                else status = "Running";
                _logger.LogInformation("Status for task {TaskId}: {Status}", id, status);
                return true;
            }

            status = "NotFound";
            _logger.LogWarning("Status requested for non-existent task: {TaskId}", id);
            return false;
        }

        public IEnumerable<(Guid Id, string Status)> GetAllTasks()
        {
            foreach (var kvp in _tasks)
            {
                TryGetStatus(kvp.Key, out var status);
                _logger.LogInformation("Task listed: {TaskId} with status {Status}", kvp.Key, status);
                yield return (kvp.Key, status);
            }
        }
    }
}