using Microsoft.AspNetCore.Mvc;
using RateLimiter.Models.Attributes;
using RateLimiter.Models.Enums;

namespace API.Controllers
{
    [ApiController]
    [Route("api/calculator")]
    public class CalculatorController : ControllerBase
    {
        // Just for tests. Make 2+ calls in a row to reproduce failed rate limit rules
        // RateLimiterOptions for rules are setup in ServiceCollectionExtensions in API project
        [RateLimiter(RateLimiterType = RateLimiterType.XRequestsPerTimespan)]
        [RateLimiter(RateLimiterType = RateLimiterType.CertainTimespanPassedSinceTheLastCall)]
        [HttpGet("sum")]
        public ActionResult<int> CalculateSum([FromQuery]int n1, [FromQuery]int n2)
        {
            var sum = n1 + n2;

            return Ok(sum);
        }
    }
}
