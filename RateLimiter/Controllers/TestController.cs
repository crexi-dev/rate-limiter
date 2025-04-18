using Microsoft.AspNetCore.Mvc;
using System;

namespace RateLimiter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                message = "API request successful!!",
                timestamp = DateTime.UtcNow
            });
        }

        [HttpGet("slow")]
        public IActionResult GetSlow()
        {
            // simulate a slow endpoint
            System.Threading.Thread.Sleep(500);
            return Ok(new
            {
                message = "Slow API request successful",
                timestamp = DateTime.UtcNow
            });
        }
    }
}