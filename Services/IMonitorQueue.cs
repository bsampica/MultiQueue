namespace MultiQueue.Services
{
    public interface IMonitorQueue
    {
        void StartMonitor();
        ValueTask MonitorAsync();
        ValueTask BuildWorkItemAsync(CancellationToken token);

    }
}
