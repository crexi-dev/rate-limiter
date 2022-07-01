using Microsoft.AspNetCore.Mvc;

namespace RateLimiter.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [RuleALimit]
        [HttpGet]
        [Route("TestA")]
        public ActionResult TestAGetInfo()
        {
            return new OkObjectResult("You have access");
        }

        [RuleBLimit]
        [HttpGet]
        [Route("TestB")]
        public ActionResult TestBGetInfo()
        {
            return new OkObjectResult("You have access");
        }

        [RuleALimit]
        [RuleBLimit]
        [HttpGet]
        [Route("TestA&B")]
        public ActionResult TestBothOfRulesGetInfo()
        {
            return new OkObjectResult("You have access");
        }
    }
}