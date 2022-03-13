using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RateLimiter.Attibutes;
using RateLimiter.BL.ServicesInterfaces;
using RateLimiter.Data.Models;
using RateLimiter.Filters;

namespace RateLimiter.Controllers
{

    [Route("[controller]")]
    [Produces("application/json")]
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        [LogRequests]
        [RaceLimiter(Data.Enums.RateLimiterTypeEnum.AutoByRegion | Data.Enums.RateLimiterTypeEnum.RequestDebounce)]

        [HttpGet("/index")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public IActionResult Index()
        {
            var result = _homeService.GetHomeAnswer();

            return Ok(new HomeResponse(result));
        }



    }
}
