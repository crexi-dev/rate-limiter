using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using RateLimiter.Models.Contract;
using RateLimiter.Services.Weather;

namespace RateLimiter.Services.Tests
{
    [TestFixture]
    public class WeatherForecastServiceTests
    {
        private IWeatherForecastService _weatherForecastService;

        [SetUp]
        public void Setup()
        {
            _weatherForecastService = new WeatherForecastService();
        }

        [Test]
        public void TestGetWeatherForecast()
        {
            //Arrange
            string[] summaries =
                {"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"};

            //Act
            var response = _weatherForecastService.GetWeatherForecast();

            //Assert
            response.Should().NotBeEmpty();
            response.Should().BeOfType(typeof(List<WeatherForecast>));
            response.Count.Should().BeInRange(1, 6);
            foreach (var forecast in response)
            {
                forecast.Summary.Should().NotBeNullOrEmpty();
                forecast.Summary.Should().ContainAny(summaries);
                forecast.TemperatureC.Should().BeInRange(-20, 56);
                forecast.Date.Should().BeOnOrAfter(DateTime.UtcNow);
            }
        }
    }
}