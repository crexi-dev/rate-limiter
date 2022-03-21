using Microsoft.AspNetCore.Mvc;

namespace RateLimiter.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RateController : ControllerBase
{
    // GET
    [HttpGet("get")]
    public IActionResult Get()
    {
        return Ok();
    }
    
    [HttpPost("post")]
    public IActionResult Post()
    {
        return Ok();
    }
}