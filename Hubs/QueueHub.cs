using Microsoft.AspNetCore.SignalR;

namespace MultiQueue.Hubs
{
    public interface IQueueClient
    {
        Task ReceiveMessage(string message);
    }

    public class QueueHub : Hub<IQueueClient>
    {
        private readonly ILogger _logger;
        public QueueHub(ILogger logger)
        {
            _logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            _logger.LogInformation($"Client Connected: ClientId: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"Client Disconnected: ClientId: {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
