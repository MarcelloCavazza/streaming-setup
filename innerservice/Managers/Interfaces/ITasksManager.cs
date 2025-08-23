namespace innerservice.Managers.Interfaces
{
    public interface ITasksManager
    {
        Guid StartTask(Func<CancellationToken, Guid, Task> work);
        
        bool TryCancelTask(Guid id);

        bool TryGetStatus(Guid id, out string status);

        IEnumerable<(Guid Id, string Status)> GetAllTasks();
    }
}