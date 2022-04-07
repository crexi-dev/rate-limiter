using NUnit.Framework;
using RateLimiter.Helpers;
using System;
using System.Threading;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        [Test]
        public void ShouldReturnTrueWhenTwoRequestsInTwoMinutes()
        {
            LimitConfigurator.AddRule("Api1", builder =>
            {
                builder.AddMaxCount(1);
                builder.AddTimeSpan(TimeSpan.FromMinutes(1));
                return builder.Build();
            });
            LimitConfigurator.AddRule("Api1", builder =>
            {
                builder.AddMaxCount(5);
                builder.AddTimeSpan(TimeSpan.FromMinutes(3));
                return builder.Build();
            });

            var validator = new RequestValidator();
            validator.IsUserAllowed("user1", "Api1");
            Thread.Sleep(TimeSpan.FromMinutes(2));
            var result = validator.IsUserAllowed("user1", "Api1");
            Assert.IsTrue(result);
        }

        [Test]
        public void ShouldReturnFalseWhenSixRequestsInTwoMinutes()
        {
            LimitConfigurator.AddRule("Api1", builder =>
            {
                builder.AddMaxCount(1);
                builder.AddTimeSpan(TimeSpan.FromMinutes(1));
                return builder.Build();
            });
            LimitConfigurator.AddRule("Api1", builder =>
            {
                builder.AddMaxCount(5);
                builder.AddTimeSpan(TimeSpan.FromMinutes(3));
                return builder.Build();
            });

            var validator = new RequestValidator();
            validator.IsUserAllowed("user1", "Api1");
            Thread.Sleep(TimeSpan.FromMinutes(2));
            validator.IsUserAllowed("user1", "Api1");
            validator.IsUserAllowed("user1", "Api1");
            validator.IsUserAllowed("user1", "Api1");
            validator.IsUserAllowed("user1", "Api1");
            var result = validator.IsUserAllowed("user1", "Api1");
            Assert.IsFalse(result);
        }

        [Test]
        public void ShouldReturnTrueWhenThreeRequestsInMoreThanTwoMinutes()
        {
            LimitConfigurator.AddRule("Api1", builder =>
            {
                builder.AddMaxCount(1);
                builder.AddTimeSpan(TimeSpan.FromMinutes(1));
                return builder.Build();
            });
            LimitConfigurator.AddRule("Api1", builder =>
            {
                builder.AddMaxCount(5);
                builder.AddTimeSpan(TimeSpan.FromMinutes(3));
                return builder.Build();
            });

            var validator = new RequestValidator();
            validator.IsUserAllowed("user1", "Api1");
            Thread.Sleep(new TimeSpan(0, 1, 1));
            validator.IsUserAllowed("user1", "Api1");
            Thread.Sleep(new TimeSpan(0, 1, 1));
            var result = validator.IsUserAllowed("user1", "Api1");
            Assert.IsTrue(result);
        }
    }
}
