using Models;

namespace innerservice.Managers.Interfaces
{
    public interface IQueuesManager
    {
        bool RemoveQueue(Guid key);

        void EnqueuePartialContent(PartialContentResponse partialContent);

        Queue<PartialContentResponse>? GetQueue(Guid queueId);
    }
}