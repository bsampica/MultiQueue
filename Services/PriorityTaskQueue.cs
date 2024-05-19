using MultiQueue.Models;

namespace MultiQueue.Services
{
    // This is the middle ware that the hosted service will communicate with
    public class PriorityTaskQueue
    {
        private readonly ILogger<PriorityTaskQueue> _logger;
        private readonly PriorityQueue<PriorityTask, int> _priorityQueue = new();

        public PriorityTaskQueue(ILogger<PriorityTaskQueue> logger)
        {
            _logger = logger;
        }

        public void Enqueue(PriorityTask task)
        {
            _priorityQueue.Enqueue(task, task.GetPriority());
        }

        public PriorityTask Dequeue() => _priorityQueue.Dequeue();

        public PriorityTask Peek() => _priorityQueue.Peek();


        public int Count() => _priorityQueue.Count;


    }
}
