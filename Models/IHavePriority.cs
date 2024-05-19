namespace MultiQueue.Models
{
    public interface IHavePriority<T>
    {
        public T GetPriority();
        public void SetPriority(T value);

        public void Enqueue(PriorityTask priorityTask);
        public PriorityTask Dequeue();
        public PriorityTask Peek();
        public int Count();
    }
}
