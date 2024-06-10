using RateLimiter.Controllers;
using RateLimiter.RateLimit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Test
{
    [TestFixture]
    public class RateLimitAttributeTests
    {
        [Test]
        public void RateLimitAttribute_IsAppliedTo_GetWeatherForecast()
        {
            // Arrange
            var controllerType = typeof(WeatherForecastController);
            var methodInfo = controllerType.GetMethod(nameof(WeatherForecastController.Get));

            // Act
            var rateLimitAttribute = methodInfo?.GetCustomAttributes(typeof(RateLimitAttribute), false).FirstOrDefault() as RateLimitAttribute;

            // Assert
            Assert.IsNotNull(rateLimitAttribute, "RateLimitAttribute is not applied to GetWeatherForecast method.");
            Assert.AreEqual(2, rateLimitAttribute.MaxRequests);
            Assert.AreEqual(5, rateLimitAttribute.TimeWindowInSeconds);
        }

        [Test]
        public void RateLimitAttribute_Properties_AreSetCorrectly()
        {
            // Arrange
            var rateLimitAttribute = new RateLimitAttribute
            {
                MaxRequests = 10,
                TimeWindowInSeconds = 60
            };

            // Act & Assert
            Assert.AreEqual(10, rateLimitAttribute.MaxRequests);
            Assert.AreEqual(60, rateLimitAttribute.TimeWindowInSeconds);
        }

        [Test]
        public void ConsumptionData_Initialization_WorksCorrectly()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var consumptionData = new ConsumptionData(now, 5);

            // Act & Assert
            Assert.AreEqual(now, consumptionData.LastResponse);
            Assert.AreEqual(5, consumptionData.NumberOfRequests);
        }

        [Test]
        public void ConsumptionData_HasConsumedAllRequests_WorksCorrectly()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var consumptionData = new ConsumptionData(now, 5);

            // Act
            var result = consumptionData.HasConsumedAllRequests(10, 5);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ConsumptionData_IncreaseRequests_WorksCorrectly()
        {
            // Arrange
            var consumptionData = new ConsumptionData(DateTime.UtcNow, 1);

            // Act
            consumptionData.IncreaseRequests(5);

            // Assert
            Assert.AreEqual(2, consumptionData.NumberOfRequests);
        }

        [Test]
        public void ConsumptionData_IncreaseRequests_ResetsAfterMaxRequests()
        {
            // Arrange
            var consumptionData = new ConsumptionData(DateTime.UtcNow, 5);

            // Act
            consumptionData.IncreaseRequests(5);

            // Assert
            Assert.AreEqual(1, consumptionData.NumberOfRequests);
        }
    }
}
