namespace MultiQueue.Services
{
    public interface IBackgroundTaskQueue
    {
        int GetQueueLength();
        ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> task);

        ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
    }
}
