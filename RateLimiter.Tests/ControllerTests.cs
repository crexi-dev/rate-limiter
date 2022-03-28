using NUnit.Framework;
using RateLimiter.API.Controllers;
using System;
using System.Net;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class ControllerTests
    {

        [Test]
        public void RestricByIpTest_Validate_RateLimit_Not_Null()
        {
            var controller = new WeatherForecastController();
            var result = controller.RestricByIp();
            TimeSpan.FromSeconds(4);       
            Assert.IsNotNull(result);
    
            
        }
    }
}
