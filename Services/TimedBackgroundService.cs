

namespace MultiQueue.Services
{
    public class TimedBackgroundService : BackgroundService
    {

        private readonly ILogger<TimedBackgroundService> _logger;
        private int _executionCount;

        public TimedBackgroundService(ILogger<TimedBackgroundService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            // When the timer should have no due-time, then do the work once now.
            DoWork();

            using PeriodicTimer timer = new(TimeSpan.FromSeconds(1));

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    DoWork();
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Timed Hosted Service is stopping.");
            }
        }

        // Could also be a async method, that can be awaited in ExecuteAsync above
        private void DoWork()
        {
            int count = Interlocked.Increment(ref _executionCount);

            _logger.LogInformation("Timed Hosted Service is working. Count: {Count}", count);
        }
    }
}
