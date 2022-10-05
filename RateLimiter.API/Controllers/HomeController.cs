using Microsoft.AspNetCore.Mvc;

namespace RateLimiter.API.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public Task<IActionResult> Index()
    {
        return Task.FromResult<IActionResult>(Ok("Access granted"));
    }
}