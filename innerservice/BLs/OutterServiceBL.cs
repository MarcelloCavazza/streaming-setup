using innerservice.BLs.Interfaces;
using innerservice.Managers.Interfaces;
using System.Text.Json;
using Models;

namespace innerservice.BLs
{
    public class OutterServiceBL : IOutterServiceBL
    {
        private readonly ITasksManager _tasksManager;
        private readonly IQueuesManager _queuesManager;

        public OutterServiceBL(ITasksManager tasksManager, IQueuesManager queuesManager)
        {
            _tasksManager = tasksManager;
            _queuesManager = queuesManager;
        }

        public Guid ExecuteCall()
        {
            var id = _tasksManager.StartTask(ExecuteCallAsync);
            
            return id;
        }

        private async Task ExecuteCallAsync(CancellationToken taskToken, Guid queueId)
        {
            try
            {
                taskToken.ThrowIfCancellationRequested();
                var url = "http://127.0.0.1:5050/stream";

                var contents = new List<string>();

                using var httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, url);

                using var response = await httpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    CancellationToken.None
                );

                taskToken.ThrowIfCancellationRequested();

                response.EnsureSuccessStatusCode();

                using var stream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(stream);

                while (!reader.EndOfStream)
                {
                    taskToken.ThrowIfCancellationRequested();
                    var line = await reader.ReadLineAsync();
                    var payload = JsonSerializer.Deserialize<Response>(line ?? "");

                    if (payload != null)
                    {
                        contents.Add(payload.Content ?? "");

                        var partialContent = new PartialContentResponse()
                        {
                            QueueGUID = queueId,
                            PartialContent = string.Join("\n", contents),
                            FetchMore = payload.FetchMore
                        };

                        taskToken.ThrowIfCancellationRequested();
                        _queuesManager.EnqueuePartialContent(partialContent);
                        contents.Clear();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _queuesManager.RemoveQueue(queueId);
            }
        }
    }
}