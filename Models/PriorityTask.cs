namespace MultiQueue.Models
{
    public class PriorityTask
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
