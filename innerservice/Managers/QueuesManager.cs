using Models;
using innerservice.Managers.Interfaces;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace innerservice.Managers
{
    public class QueuesManager : IQueuesManager
    {
        private readonly ILogger<QueuesManager> _logger;
        private ConcurrentDictionary<Guid, Queue<PartialContentResponse>> queuesDict = new ConcurrentDictionary<Guid, Queue<PartialContentResponse>>();

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

        public void RegisterQueue(PartialContentResponse @object)
        {
            var newQueue = new Queue<PartialContentResponse>();
            newQueue.Enqueue(@object);

            if (queuesDict.TryAdd(@object.QueueGUID, newQueue))
            {
                _logger.LogInformation("Queue registered: {QueueGUID}", @object.QueueGUID);
            }
            else
            {
                _logger.LogWarning("Queue registration failed: {QueueGUID}", @object.QueueGUID);
            }
        }

        public Queue<PartialContentResponse>? GetQueue(Guid queueId)
        {
            if (queuesDict.TryGetValue(queueId, out var queue))
            {
                _logger.LogInformation("Queue retrieved: {QueueId}", queueId);
                return queue;
            }
            _logger.LogWarning("Queue not found: {QueueId}", queueId);
            return queue;
        }

        public void EnqueuePartialContent(PartialContentResponse partialContent)
        {
            if (queuesDict.TryGetValue(partialContent.QueueGUID, out var queue))
            {
                lock (queue)
                {
                    queue.Enqueue(partialContent);
                    _logger.LogInformation("Enqueued content to queue: {QueueGUID}", partialContent.QueueGUID);
                }
            }
            else
            {
                RegisterQueue(partialContent);
                _logger.LogInformation("Queue created and content enqueued: {QueueGUID}", partialContent.QueueGUID);
            }
        }
    }
}