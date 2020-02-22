using NUnit.Framework;
using RateLimiter.Rules;
using System;

namespace RateLimiter.Tests.Rules
{
    [TestFixture]
    public class RulesHelperTest
    {
        [Test]
        public void ElapsedTime_Past_Config()
        {
            var config = TimeSpan.FromSeconds(10);
            var prev = DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(20));
            var elapsed = RulesHelper.HasElapsedTime(prev, config);

            Assert.IsTrue(elapsed);
        }

        [Test]
        public void ElapsedTime_Inside_Config()
        {
            var config = TimeSpan.FromSeconds(10);
            var prev = DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(5));
            var elapsed = RulesHelper.HasElapsedTime(prev, config);

            Assert.IsFalse(elapsed);
        }
    }
}