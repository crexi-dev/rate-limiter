using NUnit.Framework;
using RateLimiter;
using System;
using System.Collections.Generic;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class CacheHelperTests
    {
        private CacheHelper _cacheHelper;

        [SetUp]
        public void SetUp()
        {
            _cacheHelper = new CacheHelper();
        }

        [Test]
        public void LastRequestTime_ShouldReturnMaxValue_WhenNoRequests()
        {
            // Act
            var result = _cacheHelper.LastRequestTime("test-key");

            // Assert
            Assert.AreEqual(TimeSpan.MaxValue, result);
        }

        [Test]
        public void LastRequestTime_ShouldReturnTimeSinceLastRequest_WhenRequestsExist()
        {
            // Arrange
            var key = "test-key";
            _cacheHelper.AddRequest(key);
            System.Threading.Thread.Sleep(1000); // Wait for 1 second

            // Act
            var result = _cacheHelper.LastRequestTime(key);

            // Assert
            Assert.IsTrue(result.TotalMilliseconds >= 1000);
        }

        [Test]
        public void RequestsCount_ShouldReturnZero_WhenNoRequests()
        {
            // Act
            var result = _cacheHelper.RequestsCount("test-key", TimeSpan.FromSeconds(1));

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void RequestsCount_ShouldReturnCorrectCount_WhenRequestsExist()
        {
            // Arrange
            var key = "test-key";
            _cacheHelper.AddRequest(key);
            _cacheHelper.AddRequest(key);
            System.Threading.Thread.Sleep(1000); // Wait for 1 second

            // Act
            var result = _cacheHelper.RequestsCount(key, TimeSpan.FromSeconds(2));

            // Assert
           Assert.AreEqual(2, result);
        }

        [Test]
        public void AddRequest_ShouldAddRequestToCache()
        {
            // Arrange
            var key = "test-key";

            // Act
            _cacheHelper.AddRequest(key);
            var result = _cacheHelper.RequestsCount(key, TimeSpan.FromSeconds(1));

            // Assert
            Assert.AreEqual(1, result);
        }
    }
}
