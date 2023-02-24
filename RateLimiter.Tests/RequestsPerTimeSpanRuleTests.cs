using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RateLimiter.Rules;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RequestsPerTimeSpanRuleTests
    {
        [Test]
        public void Success()
        {
            var rule = new RequestsPerTimeSpanRule
            {
                RequestsCount = 5,
                TimeSpan = TimeSpan.FromSeconds(5)
            };

            var now = DateTime.Today;

            Assert.That(rule.Check(new[]
            {
                now,
                now.AddSeconds(1),
                now.AddSeconds(2),
                now.AddSeconds(3),
                now.AddSeconds(4),
                now.AddSeconds(5),
                now.AddSeconds(6),
            }), Is.False);
        }

        [Test]
        public void Fail()
        {
            var rule = new RequestsPerTimeSpanRule
            {
                RequestsCount = 4,
                TimeSpan = TimeSpan.FromSeconds(5)
            };

            var now = DateTime.Today;

            Assert.That(rule.Check(new[]
            {
                now, 
                now.AddSeconds(1),
                now.AddSeconds(2),
                now.AddSeconds(3),
                now.AddSeconds(4),
                now.AddSeconds(5),
                now.AddSeconds(6),
            }), Is.False);
        }
    }
}
