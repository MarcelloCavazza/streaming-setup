using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using innerservice.Managers.Interfaces;

namespace innerservice.Managers
{
    public class TasksManager
    { 
        private readonly IQueuesManager _queuesManager;

        public TasksManager(IQueuesManager queuesManager)
        {
            _queuesManager = queuesManager;
        }

        private readonly ConcurrentDictionary<Guid, (Task Task, CancellationTokenSource Cts)> _tasks = new();

        public Guid StartTask(Func<CancellationToken, Guid, Task> work)
        {
            var cts = new CancellationTokenSource();
            var id = Guid.NewGuid();

            var task = Task.Run(() => work(cts.Token, id), cts.Token);

            _tasks[id] = (task, cts);

            // Remove task from dictionary when completed
            _ = task.ContinueWith(_ => _tasks.TryRemove(id, out (Task, CancellationTokenSource) _));

            return id;
        }
        
        public bool TryCancelTask(Guid id)
        {
            if (_tasks.TryGetValue(id, out var entry))
            {
                entry.Cts.Cancel();
                _queuesManager.RemoveQueue(id.ToString());
                return true;
            }
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
                return true;
            }

            status = "NotFound";
            return false;
        }

        public IEnumerable<(Guid Id, string Status)> GetAllTasks()
        {
            foreach (var kvp in _tasks)
            {
                TryGetStatus(kvp.Key, out var status);
                yield return (kvp.Key, status);
            }
        }
    }
}