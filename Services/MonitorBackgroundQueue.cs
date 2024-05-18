
namespace MultiQueue.Services
{
    public class MonitorBackgroundQueue : IMonitorQueue
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<MonitorBackgroundQueue> _logger;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly CancellationToken _cancellationToken;

        public MonitorBackgroundQueue(IBackgroundTaskQueue taskQueue,
            ILogger<MonitorBackgroundQueue> logger,
            IHostApplicationLifetime applicationLifetime)
        {
            _taskQueue = taskQueue;
            _logger = logger;
            _applicationLifetime = applicationLifetime;
        }
        public void StartMonitor()
        {
            _logger.LogInformation("MonitorAsync Loop is starting");

            // Run a console user input loop in a background thread
            Task.Run(async () => await MonitorAsync());
        }

        public async ValueTask MonitorAsync()
        {
            _logger.LogInformation("Monitor Async Called");
            while (!_cancellationToken.IsCancellationRequested)
            {
                var keyStroke = Console.ReadKey();

                if (keyStroke.Key == ConsoleKey.W)
                {
                    // Enqueue a background work item
                    _logger.LogInformation($"Queuing new Work Item: QueueLength: {_taskQueue.GetQueueLength()}");
                    await _taskQueue.QueueBackgroundWorkItemAsync(BuildWorkItemAsync);
                }
            }
        }

        public async ValueTask BuildWorkItemAsync(CancellationToken token)
        {
            // Simulate three 5-second tasks to complete
            // for each enqueued work item

            int delayLoop = 0;
            var guid = Guid.NewGuid().ToString();

            _logger.LogInformation("Queued Background Task {Guid} is starting.", guid);

            while (!token.IsCancellationRequested && delayLoop < 3)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), token);
                }
                catch (OperationCanceledException)
                {
                    // Prevent throwing if the Delay is cancelled
                }

                delayLoop++;

                _logger.LogInformation("Queued Background Task {Guid} is running. "
                                       + "{DelayLoop}/3", guid, delayLoop);
            }

            if (delayLoop == 3)
            {
                _logger.LogInformation("Queued Background Task {Guid} is complete.", guid);
            }
            else
            {
                _logger.LogInformation("Queued Background Task {Guid} was cancelled.", guid);
            }
        }
    }
}
