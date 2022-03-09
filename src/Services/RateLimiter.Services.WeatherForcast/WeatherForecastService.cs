using RateLimiter.Models.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Services.Weather
{
    /// <summary>
    /// Weather forecast service
    /// </summary>
    public class WeatherForecastService : IWeatherForecastService
    {
        private readonly Random _random;
        private readonly string[] _summaries;

        public WeatherForecastService()
        {
            _random = new Random();
            _summaries = new[]
                {"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"};
        }

        /// <summary>
        /// A method that returns a randomly generated weather forecast
        /// </summary>
        public IList<WeatherForecast> GetWeatherForecast() => Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast(DateTime.Now.AddDays(index), _random.Next(-20, 55),
                _summaries[_random.Next(_summaries.Length)])).ToList();
    }
}
