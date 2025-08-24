using Models.InnerService.Enums;

namespace innerservice.Managers.Interfaces
{
    public interface ITasksManager
    {
        Guid StartTask(Func<CancellationToken, Guid, Task> work);
        
        bool TryCancelTask(Guid id);

        bool TryGetStatus(Guid id, out TaskStatusEnum status);

        IEnumerable<(Guid Id, TaskStatusEnum Status)> GetAllTasks();
    }
}