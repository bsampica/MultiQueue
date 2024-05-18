
using MultiQueue.Services;

namespace MultiQueue;

public class Program
{
    private static readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();

        // Setup the background queue and monitor
        builder.Services.AddSingleton<MonitorBackgroundQueue>();
        builder.Services.AddSingleton<IBackgroundTaskQueue>(ctx =>
        {
            var queueCapacity = 100;
            return new BackgroundTaskQueue(queueCapacity);
        });

        // Add the hosted services
        builder.Services.AddHostedService<QueueHostedService>();


        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();




        var app = builder.Build();

        //var queueService = app.Services.GetRequiredService<QueueService>();
        //queueService.StartAsync(_cancellationTokenSource.Token);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();
        app.MapControllers();

        var monitorLoop = app.Services.GetRequiredService<MonitorBackgroundQueue>();
        monitorLoop.StartMonitor();

        app.Run();


    }
}
