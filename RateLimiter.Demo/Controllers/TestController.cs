using Microsoft.AspNetCore.Mvc;

namespace RateLimiter.Demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Success");
        }
    }
}
