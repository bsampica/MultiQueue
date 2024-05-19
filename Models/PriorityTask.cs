namespace MultiQueue.Models
{
    public class PriorityTask : IHavePriority<int>
    {
        public Guid TaskId { get; private set; }
        public Action? TaskWork { get; set; }


        public PriorityTask()
        {
            TaskId = Guid.NewGuid();
        }
        public int GetPriority()
        {
            return 0;
        }
    }
}
