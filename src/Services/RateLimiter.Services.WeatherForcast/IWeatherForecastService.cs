using System.Collections.Generic;
using RateLimiter.Models.Contract;

namespace RateLimiter.Services.Weather
{
    public interface IWeatherForecastService
    {
        IList<WeatherForecast> GetWeatherForecast();
    }
}
