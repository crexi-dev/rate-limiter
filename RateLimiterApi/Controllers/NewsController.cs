using Microsoft.AspNetCore.Mvc;
using RateLimiter.Decorators;

namespace RateLimiterApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewsController : ControllerBase
    {  
        [RateLimitRequest(Type = RateLimiter.Types.RateLimiterType.NumberRequestsPerTime, MaxRequests = 5, TimeWindowSeconds = 5)]
        [HttpGet("news-1")]
        public ActionResult<string> GetNews1()
        {
            return "News 1!";
        }

        [RateLimitRequest(Type = RateLimiter.Types.RateLimiterType.NumberRequestsPerTime, MaxRequests = 100, TimeWindowSeconds = 1)]
        [RateLimitRequest(Type = RateLimiter.Types.RateLimiterType.TimeBetweenTwoRequests, TimeWindowSeconds = 5, Regions = new string[] { "eu-west-1"})]
        [HttpGet("news-2")]
        public ActionResult<string> GetNews2()
        {
            return "News 2!";
        }

        [RateLimitRequest(Type = RateLimiter.Types.RateLimiterType.TimeBetweenTwoRequests, TimeWindowSeconds = 5, Regions = new string[] { "us-east-1" })]
        [HttpGet("news-3")]
        public ActionResult<string> GetNews3()
        {
            return "News 3!";
        }
    }
}