using Microsoft.AspNetCore.Mvc;
using RateLimiter.Decorators;

namespace RateLimiterApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [RateLimitRequest(Type = RateLimiter.Types.RateLimiterType.NumberRequestsPerTime, MaxRequests = 5, TimeWindowSeconds = 5)]
    public class TopicsController : ControllerBase
    {        
        [HttpGet("topic-1")]
        public ActionResult<string> GetTopic1()
        {
            return "Topic 1!";
        }

        [HttpGet("topic-2")]
        public ActionResult<string> GetTopic2()
        {
            return "Topic 2!";
        }

        [HttpGet("topic-3")]
        public ActionResult<string> GetTopic3()
        {
            return "Topic 3!";
        }
    }
}
