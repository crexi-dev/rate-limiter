
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RateLimiter.Configuration;
using System;

namespace RateLimiter.Tests.Configuration
{
    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        public void DeserializationTest()
        {
            var path = $@"{AppDomain.CurrentDomain.BaseDirectory}\TestData\appsettings.json";

            var configurationBuilder = new ConfigurationBuilder().AddJsonFile(path);
            var configuration = configurationBuilder.Build();

            var target = configuration.GetSection("RateLimiting").Get<RateLimitConfiguration>();

            var rule0Parameters = target.Endpoints[0].Rules[0].Parameters;
            var rule1Parameters = target.Endpoints[0].Rules[1].Parameters;

            Assert.AreEqual(1, target.Endpoints.Length);
            Assert.AreEqual("/WeatherForecast", target.Endpoints[0].PathPattern);
            Assert.AreEqual((HttpVerbFlags)31, target.Endpoints[0].Verbs);
            Assert.AreEqual("RequestsPerWindowRateLimitRule", target.Endpoints[0].Rules[0].Type);
            Assert.AreEqual("CadenceRateLimitRule", target.Endpoints[0].Rules[1].Type);
            Assert.AreEqual(Convert.ToUInt32(rule0Parameters["WindowMs"]), 60000u);
            Assert.AreEqual(Convert.ToUInt32(rule0Parameters["RequestLimit"]), 100u);
            Assert.AreEqual(Convert.ToUInt32(rule1Parameters["MinimumDelayBetweenRequestsMs"]), 1000u);
        }
    }
}
