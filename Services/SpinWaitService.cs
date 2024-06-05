namespace MultiQueue.Services
{
    public class SpinWaitService
    {
        public SpinWaitService() { }

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
            var targetTime = DateTime.Now.Add(duration);
            while (DateTime.Now <= targetTime)
            {
                Thread.Sleep(100);  // slow it down a little bit
                Console.WriteLine("WAITING LOOP");
            }
        }

        private async Task HandleInternalSpinAsync(TimeSpan duration)
        {
            var targetTime = DateTime.Now.Add(duration);
            while (DateTime.Now <= targetTime)
            {
                await Task.Delay(300);
                Console.WriteLine("ASYNC LOOP WAIT");
            }
        }

    }
}