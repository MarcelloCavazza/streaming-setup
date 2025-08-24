using Models.InnerService.Responses.Queues;

namespace innerservice.Managers.Interfaces
{
    public interface IQueuesManager
    {
        bool RemoveQueue(Guid key);

        void EnqueuePartialContent(QueueContent partialContent);

        Queue<QueueContent>? GetQueue(Guid queueId);
    }
}