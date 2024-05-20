
using Hangfire;
using MultiQueue.Services;
using System.Globalization;

namespace MultiQueue;

public class Program
{
    private static readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();

        // Setup Hangfire
        builder.Services.AddHangfire(config =>
        {
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseColouredConsoleLogProvider()
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseIgnoredAssemblyVersionTypeResolver()
            .UseRecommendedSerializerSettings()
            .UseResultsInContinuations()
            .UseDefaultCulture(CultureInfo.CurrentCulture)
            .UseSqlServerStorage(
                builder.Configuration.GetConnectionString("HangfireConnection"));
        });

        builder.Services.AddHangfireServer(options =>
        {
            // options.IsLightweightServer = true;
            options.WorkerCount = Environment.ProcessorCount * 20;
            // options.ServerTimeout = TimeSpan.FromSeconds(30);
            // options.ServerCheckInterval = TimeSpan.FromSeconds(30);
        });


        // Setup the background queue and monitor
        // builder.Services.AddSingleton<MonitorBackgroundQueue>();
        // builder.Services.AddSingleton<IBackgroundTaskQueue>(ctx =>
        // {
        //    var queueCapacity = 100;
        //    return new BackgroundTaskQueue(queueCapacity);
        // });

        // Add the hosted services
        // builder.Services.AddHostedService<QueueHostedService>();
        // builder.Services.AddHostedService<TimedBackgroundService>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        var app = builder.Build();

        // var queueService = app.Services.GetRequiredService<QueueService>();
        // queueService.StartAsync(_cancellationTokenSource.Token);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();
        app.MapControllers();
        app.UseHangfireDashboard();

        // var monitorLoop = app.Services.GetRequiredService<MonitorBackgroundQueue>();
        // monitorLoop.StartMonitor();

        app.Run();


    }
}
