using Microsoft.AspNetCore.Mvc;

namespace MultiQueue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueueController(ILogger<QueueController> logger) : ControllerBase
    {
        [HttpGet(Name = "GetQueueItems", Order = 0)]
        public IEnumerable<string> Get()
        {
            logger.LogDebug("Called: /api/queue/GET");
            return Enumerable.Range(1, 5).Select(index =>
            {
                return $"Queue:{index}";
            });
        }
    }
}
