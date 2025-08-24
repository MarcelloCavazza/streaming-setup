using Models.InnerService.Responses.Queues;
using innerservice.Managers.Interfaces;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace innerservice.Managers
{
    public class QueuesManager : IQueuesManager
    {
        private readonly ILogger<QueuesManager> _logger;
        private ConcurrentDictionary<Guid, Queue<QueueContent>> queuesDict = new ConcurrentDictionary<Guid, Queue<QueueContent>>();

        public QueuesManager(ILogger<QueuesManager> logger)
        {
            _logger = logger;
        }

        public bool RemoveQueue(Guid key)
        {
            var success = false;

            if (queuesDict.TryRemove(key, out _))
            {
                _logger.LogInformation("Queue removed: {Key}", key);
                success = true;
            }
            else
            {
                _logger.LogWarning("Could not remove queue: {Key}", key);
            }

            return success;
        }

        public void RegisterQueue(QueueContent @object)
        {
            var newQueue = new Queue<QueueContent>();
            newQueue.Enqueue(@object);

            if (queuesDict.TryAdd(@object.QueueId, newQueue))
            {
                _logger.LogInformation("Queue registered: {QueueId}", @object.QueueId);
            }
            else
            {
                _logger.LogWarning("Queue registration failed: {QueueId}", @object.QueueId);
            }
        }

        public Queue<QueueContent>? GetQueue(Guid queueId)
        {
            if (queuesDict.TryGetValue(queueId, out var queue))
            {
                _logger.LogInformation("Queue retrieved: {QueueId}", queueId);
                return queue;
            }
            _logger.LogWarning("Queue not found: {QueueId}", queueId);
            return queue;
        }

        public void EnqueuePartialContent(QueueContent partialContent)
        {
            if (queuesDict.TryGetValue(partialContent.QueueId, out var queue))
            {
                lock (queue)
                {
                    queue.Enqueue(partialContent);
                    _logger.LogInformation("Enqueued content to queue: {QueueId}", partialContent.QueueId);
                }
            }
            else
            {
                RegisterQueue(partialContent);
                _logger.LogInformation("Queue created and content enqueued: {QueueId}", partialContent.QueueId);
            }
        }
    }
}