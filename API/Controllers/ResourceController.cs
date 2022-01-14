using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class ResourceController : ControllerBase
{
    [HttpGet]
    [Route("[action]")]
    public IActionResult Get()
    {
        return Ok();
    }
}