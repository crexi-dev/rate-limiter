using NUnit.Framework;
using RateLimiter.Interfaces;
using RateLimiter.Rules;
using System.Collections.Generic;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        InMemoryStore store = new InMemoryStore();
        [SetUp]
        public void Init()
        { store.ClearStorage(); }

        [TearDown]
        public void Cleanup()
        { store.ClearStorage(); }

        [Test, Order(1)]
        public void Check_RequestRateValidator_OK()
        {
            List<IRateLimiter> limiters = new();
            limiters.Add(new RequestRateRule());

            RequestValidatorBuilder request = new(limiters);

            Assert.IsTrue(request.ValidateClientToken("avcc"));
        }

        [Test, Order(2)]
        public void Check_RequestRateValidator_OK_5Times()
        {
            List<IRateLimiter> limiters = new();
            limiters.Add(new RequestRateRule());
            bool result = false;
            RequestValidatorBuilder request = new(limiters);

            for (int i = 0; i < 5; i++)
                result = request.ValidateClientToken("avcc");
            
            Assert.IsTrue(result);
        }

        [Test, Order(3)]
        public void Check_RequestRateValidator_NOTOK()
        {
            List<IRateLimiter> limiters = new();
            limiters.Add(new RequestRateRule());

            RequestValidatorBuilder request = new(limiters);
            bool result = false;
            for (int i = 0; i <= 6; i++)
                result = request.ValidateClientToken("avcc");

            Assert.IsFalse(result);
        }

        [Test, Order(4)]
        public void Check_RequestRateValidator_EmptyInput()
        {
            List<IRateLimiter> limiters = new();
            limiters.Add(new RequestRateRule());

            RequestValidatorBuilder request = new(limiters);
            bool result = request.ValidateClientToken(string.Empty);

            Assert.IsFalse(result);
        }
    }
}
