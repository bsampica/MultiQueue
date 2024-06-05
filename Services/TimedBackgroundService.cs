

using Hangfire;
using Hangfire.Server;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Diagnostics;

namespace MultiQueue.Services
{
    public class TimedBackgroundService : BackgroundService
    {
        private readonly IBackgroundJobClient _jobClient;
        private readonly ILogger<TimedBackgroundService> _logger;
        private int _executionCount;
        private readonly IServiceProvider _serviceProvider;
        private CancellationToken tokenReference;


        public TimedBackgroundService(ILogger<TimedBackgroundService> logger, IBackgroundJobClient backgroundJobClient, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _jobClient = backgroundJobClient;
            _serviceProvider = serviceProvider;
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is Starting.");

            tokenReference = stoppingToken;
            PrintStartMessage();
            return Task.CompletedTask;
        }

        // Could also be a async method, that can be awaited in ExecuteAsync above
        private void RunJobScheduler()
        {
            int count = Interlocked.Increment(ref _executionCount);
            _logger.LogInformation($"Timed Hosted Service is working. Count: {count}");
            _logger.LogInformation("Scheduling 5000 jobs, each takes  2 seconds to run");

            for (int i = 0; i <= 10000; i++)
            {
                var job1 = _jobClient.Enqueue<TimedBackgroundService>(x => x.WorkerFromHangfire(i));
                var job2 = _jobClient.ContinueJobWith<TimedBackgroundService>(job1, x => x.DoSomeLongWork(null));
            }

            _logger.LogInformation("Scheduling 500 Jobs....complete!");
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
            _logger.LogInformation($"Long running task is starting. {DateTime.Now}: Finish expected in 60 seconds");


            await Task.Delay(TimeSpan.FromSeconds(30));

            _logger.LogInformation($"Long running task is complete. {DateTime.Now}");
        }

        private void PrintAddress()
        {
            _logger.LogInformation("Checking Addresses");
            var server = _serviceProvider.GetRequiredService<IServer>();
            var addressFeature = server.Features.Get<IServerAddressesFeature>();
            foreach (var address in addressFeature?.Addresses!)
            {
                _logger.LogInformation($"Listening on: {address}");
            }

        }

        public void PrintStartMessage()
        {
            var lifetime = _serviceProvider.GetRequiredService<IHostApplicationLifetime>();
            lifetime.ApplicationStarted.Register(() =>
            {
                _logger.LogInformation("Application Started, by triggered event in Timed Background Service");
                PrintAddress();
                Task.Run(async () => await CalculatePrimeNumbers(1, 1_000_000));
            });
        }
        public void StopApplication()
        {
            _logger.LogInformation("StopApplication called in TimedBackgroundService");
        }

        private Task CalculatePrimeNumbers(int startNumber, int endNumber)
        {
            Stopwatch sw = new();
            sw.Start();
            var primeNumbers = new List<int>();
            _logger.LogInformation($"Running CPU Intensive Work..." +
                $"Calculating Prime Numbers between {startNumber} and {endNumber}");

            for (int i = startNumber; i <= endNumber; i++)
            {
                int counter = 0;
                for (int j = 2; j <= i / 2; j++)
                {
                    if (i % j == 0)
                    {
                        counter++;
                        return Task.FromException(new Exception("Error in calculation"));
                    }
                }
                if (counter == 0 && i != 1)
                {
                    primeNumbers.Add(i);
                }
            }
            _logger.LogInformation($"Calculation Done, found {primeNumbers.Count} " +
                $"prime numbers, the last is {primeNumbers.Last()}, calculation took: {FormatMilliseconds(sw.Elapsed.TotalMilliseconds)}");

            return Task.CompletedTask;
        }

        private string FormatMilliseconds(double ms)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(ms);
            return string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                t.Hours,
                t.Minutes,
                t.Seconds,
                t.Milliseconds);
        }
    }
}
