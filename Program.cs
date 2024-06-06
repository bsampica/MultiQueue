
using Hangfire;
using MultiQueue.Hubs;
using MultiQueue.Services;
using System.Globalization;

namespace MultiQueue;

public class Program
{
    private static readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public static async Task Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddSignalR(options => options.EnableDetailedErrors = true);

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
            options.WorkerCount = 300;

        });

        // builder.Services.AddHostedService<TimedBackgroundService>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        var spinWait = new SpinWaitService();
       
        // This should be a blocking call
        spinWait.SpinUntil(TimeSpan.FromSeconds(10));

        // This should be a non-blocking call
        _ = spinWait.SpinUntilAsync(TimeSpan.FromSeconds(10));
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();
        app.MapControllers();
        app.UseHangfireDashboard();
        app.MapHub<QueueHub>("/queuehub");

        app.Run();


    }
}
