using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RateLimiter.Common.Utilities;
using RateLimiter.Models.Contract;
using RateLimiter.Api.Attributes;
using RateLimiter.Services.Weather;

namespace RateLimiter.Api.Controllers
{
    /// <summary>
    /// Weather forecast controller
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class WeatherForecastController : Controller
    {
        private readonly IWeatherForecastService _service;

        public WeatherForecastController(IWeatherForecastService service)
        {
            _service = service;
        }

        [CacheClientMetrics]
        [MaxRequestsThrottle]
        [MaxSizeThrottle]
        [MaxRequestUnitsThrottle]
        [SaveClientMetrics]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WeatherForecastResponse))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ErrorResponse))]
        public IActionResult GetWeatherForecast()
        {
            var weatherForecast = _service.GetWeatherForecast();
            var size = weatherForecast.ToByteArray().Length;
            var consistencyLevel = Helper.GenerateRandomConsistencyLevel();
            var requestUnits = Helper.CalculateRequestUnits(size, consistencyLevel);

            return Ok(new WeatherForecastResponse(weatherForecast.Count, size, requestUnits, weatherForecast));
        }
    }
}
