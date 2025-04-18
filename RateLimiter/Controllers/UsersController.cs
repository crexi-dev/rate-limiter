using Microsoft.AspNetCore.Mvc;
using System;

namespace RateLimiter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                message = "Users API request successful!!",
                timestamp = DateTime.UtcNow
            });
        }
    }
}