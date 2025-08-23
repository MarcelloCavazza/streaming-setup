using Models;

namespace innerservice.Managers.Interfaces
{
    public interface IQueuesManager
    {
        void RemoveQueue(string key);

        void EnqueuePartialContent(PartialContentResponse partialContent);

        Queue<PartialContentResponse>? GetQueue(string queueId);
    }
}