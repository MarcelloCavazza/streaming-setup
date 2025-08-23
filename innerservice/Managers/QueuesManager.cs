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
        
        public void EnqueuePartialContent(PartialContentResponse partialContent)
        {
            if (!queuesDict.ContainsKey(partialContent.QueueGUID))
            {
                RegisterQueue(partialContent);
            }




        }
        
    }
}