using Models;
using innerservice.Managers.Interfaces;
using System.Collections.Concurrent;

namespace innerservice.Managers
{
    public class QueuesManager : IQueuesManager
    {
        private ConcurrentDictionary<string, Queue<PartialContentResponse>> queuesDict = new ConcurrentDictionary<string, Queue<PartialContentResponse>>();

        public void RemoveQueue(string key)
        {
            queuesDict.TryRemove(key, out _);
        }

        public void RegisterQueue(PartialContentResponse @object)
        {
            var newQueue = new Queue<PartialContentResponse >();
            newQueue.Enqueue(@object);

            queuesDict.TryAdd(@object.QueueGUID, newQueue);
        }

        public Queue<PartialContentResponse>? GetQueue(string queueId)
        {
            queuesDict.TryGetValue(queueId, out var queue);
            return queue;
        }

        public void EnqueuePartialContent(PartialContentResponse partialContent)
        {
            if (queuesDict.TryGetValue(partialContent.QueueGUID, out var queue))
            {
                lock (queue)
                {
                    queue.Enqueue(partialContent);
                }
            }
            else
            {
                RegisterQueue(partialContent);
            }
        }
    }
}