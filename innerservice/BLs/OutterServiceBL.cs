using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            var id = _tasksManager.StartTask(async (taskToken, id) =>
            {
                await ExecuteCallAsync(taskToken, id);
            });
            
            return id;
        }

        private async Task ExecuteCallAsync(CancellationToken taskToken, Guid queueId)
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
            using var reader = new System.IO.StreamReader(stream);

            var startTime = DateTime.Now;
            var endTime = startTime.AddSeconds(1);

            while (!reader.EndOfStream)
            {
                taskToken.ThrowIfCancellationRequested();
                var line = await reader.ReadLineAsync();
                    var payload = JsonSerializer.Deserialize<Response>(line ?? "");
                if (payload != null)
                {
                    contents.Add(payload.Content ?? "");

                    if (DateTime.Now >= endTime)
                    {
                        startTime = DateTime.Now;
                        endTime = startTime.AddSeconds(1);

                        var partialContent = new PartialContentResponse()
                        {
                            QueueGUID = queueId.ToString(),
                            PartialContent = String.Join("\n", contents),
                            FetchMore = payload.FetchMore
                        };

                        taskToken.ThrowIfCancellationRequested();
                        _queuesManager.EnqueuePartialContent(partialContent);
                        contents.Clear();
                    }
                }

            }

            if (contents.Count > 0)
            {
                var partialContent = new PartialContentResponse()
                {
                    QueueGUID = queueId.ToString(),
                    PartialContent = String.Join("\n", contents),
                    FetchMore = false
                };

                taskToken.ThrowIfCancellationRequested();
                _queuesManager.EnqueuePartialContent(partialContent);
            }
        }
    }
}