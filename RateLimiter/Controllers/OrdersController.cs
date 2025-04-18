using Microsoft.AspNetCore.Mvc;
using System;

namespace RateLimiter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                message = "Orders API request successful!!",
                timestamp = DateTime.UtcNow
            });
        }

        [HttpGet("item")]
        public IActionResult GetItem()
        {
            return Ok(new
            {
                message = "Orders item API request successful!!",
                timestamp = DateTime.UtcNow
            });
        }
    }
}