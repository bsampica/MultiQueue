namespace MultiQueue.Services
{
    public class SpinWaitService
    {
        public void SpinUntil(TimeSpan duration)
        {
            HandleInternalSpin(duration);
        }

        public async Task SpinUntilAsync(TimeSpan duration)
        {
            await HandleInternalSpinAsync(duration);
        }

        private void HandleInternalSpin(TimeSpan duration)
        {
            Console.WriteLine("Starting SPINWAIT - Blocking Call");
            var targetTime = DateTime.Now.Add(duration);
            while (DateTime.Now <= targetTime)
            {
                Thread.Sleep(100);  // slow it down a little bit
                Console.WriteLine("SYNC LOOP WAIT");
            }
            Console.WriteLine("Ending SPINWAIT - Blocking Call (unblocked)");
        }

        private async Task HandleInternalSpinAsync(TimeSpan duration)
        {
            Console.WriteLine("Starting SPINWAIT ASYNC - NonBlocking Call");
            var targetTime = DateTime.Now.Add(duration);
            while (DateTime.Now <= targetTime)
            {
                await Task.Delay(300);
                Console.WriteLine("ASYNC LOOP WAIT");
            }

            Console.WriteLine("Ending SPINWAIT - NonBlocking Call (continue)");
        }

    }
}