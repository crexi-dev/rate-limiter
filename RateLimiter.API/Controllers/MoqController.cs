using Microsoft.AspNetCore.Mvc;
using RateLimiter.Attributes;

namespace RateLimiter.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MoqController : ControllerBase
    {
        [HttpGet]
        [LimitWithinTimespan(AllowedRequestCount = 5, PerMinutes = 1)]
        public string Test()
        {
            return "Hello Moq";
        }
    }
}
