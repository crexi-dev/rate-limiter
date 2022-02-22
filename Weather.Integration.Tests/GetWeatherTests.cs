using RateLimits.History;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace RateLimits.Integration.Tests
{
    public class GetWeatherTests
    {
        [Fact]
        public async Task RequestsPerTimespanRule()
        {
            using var application = new WeatherApi();

            var client = application.CreateClient();

            var firstTry = await client.GetAsync("/WeatherForecast?token=testtoken&region=Georgia");
            var secondTry = await client.GetAsync("/WeatherForecast?token=testtoken&region=Georgia");
            var thirdTry = await client.GetAsync("/WeatherForecast?token=testtoken&region=Georgia");

            Assert.Equal(HttpStatusCode.OK, firstTry.StatusCode);
            Assert.Equal(HttpStatusCode.OK, secondTry.StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, thirdTry.StatusCode);
        }

        [Fact]
        public async Task TimespanSinceLastCallRule()
        {
            using var application = new WeatherApi();

            var client = application.CreateClient();

            var firstTry = await client.GetAsync("/WeatherForecast?token=testtoken&region=USA");
            var secondTry = await client.GetAsync("/WeatherForecast?token=testtoken&region=USA");

            Assert.Equal(HttpStatusCode.OK, firstTry.StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, secondTry.StatusCode);
        }
    }
}
