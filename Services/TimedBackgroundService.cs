﻿

using Hangfire;
using Hangfire.Server;

namespace MultiQueue.Services
{
    public class TimedBackgroundService : BackgroundService
    {
        private readonly IBackgroundJobClient _jobClient;
        private readonly ILogger<TimedBackgroundService> _logger;
        private int _executionCount;

        public TimedBackgroundService(ILogger<TimedBackgroundService> logger, IBackgroundJobClient backgroundJobClient)
        {
            _logger = logger;
            _jobClient = backgroundJobClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            // When the timer should have no due-time, then do the work once now.
            RunJobScheduler();

            using PeriodicTimer timer = new(TimeSpan.FromMinutes(10));

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    RunJobScheduler();
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Timed Hosted Service is stopping.");
            }
        }

        // Could also be a async method, that can be awaited in ExecuteAsync above
        private void RunJobScheduler()
        {
            int count = Interlocked.Increment(ref _executionCount);
            _logger.LogInformation("Timed Hosted Service is working. Count: {Count}", count);
            _logger.LogInformation("Scheduling a bunch of jobs, each takes 0.5 seconds to run");

            for (int i = 0; i <= 5000; i++)
            {
                var job1 = _jobClient.Enqueue<TimedBackgroundService>(x => x.WorkerFromHangfire(i));
                var job2 = _jobClient.ContinueJobWith<TimedBackgroundService>(job1, x => x.DoSomeLongWork(null));
            }


        }

        [ContinuationsSupport(pushResults: true)]
        public int WorkerFromHangfire(int index)
        {
            Thread.Sleep(2000);
            Console.WriteLine($"Did something cool, Hangfire Job: {index} - Ran at {DateTime.Now.ToShortTimeString()}");
            return index;
        }


        public async Task DoSomeLongWork(PerformContext? context)
        {
            var jobParameter = context?.GetJobParameter<int>("AntecedentResult");

            _logger.LogInformation($"Context Received: {context?.BackgroundJob.Id}:{context?.Items.ToString()}");
            _logger.LogInformation($"Job Parameters: {jobParameter}");
            _logger.LogInformation($"Long running task is starting. {DateTime.Now}: Finish expected in 10 seconds");


            await Task.Delay(TimeSpan.FromSeconds(20));

            _logger.LogInformation($"Long running task is complete. {DateTime.Now}");
        }
    }
}
