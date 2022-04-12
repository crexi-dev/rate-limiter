using NUnit.Framework;
using RateLimiter.Interfaces;
using RateLimiter.Repository;
using RateLimiter.Rules;
using System.Collections.Generic;
using System.Threading;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        private readonly IRepository _repository;
        public RateLimiterTest()
        {
             _repository = new InMemoryStorage();
        }
        [SetUp]
        public void Init()
        { _repository.ClearStorage(); }

        [TearDown]
        public void Cleanup()
        { _repository.ClearStorage(); }

        [Test, Order(1)]
        public void Check_RequestRateValidator_OK()
        {
            List<IRateLimiter> limiters = new();
            limiters.Add(new RequestRateRule());

            RuleValidatorBuilder request = new(limiters);

            Assert.IsTrue(request.ValidateClientToken("avcc"));
        }

        [Test, Order(2)]
        public void Check_RequestRateValidator_OK_5Times()
        {
            List<IRateLimiter> limiters = new();
            limiters.Add(new RequestRateRule());
            bool result = false;
            RuleValidatorBuilder request = new(limiters);

            for (int i = 0; i < 5; i++)
                result = request.ValidateClientToken("avcc");
            
            Assert.IsTrue(result);
        }

        [Test, Order(3)]
        public void Check_RequestRateValidator_NOTOK()
        {
            List<IRateLimiter> limiters = new();
            limiters.Add(new RequestRateRule());

            RuleValidatorBuilder request = new(limiters);
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

            RuleValidatorBuilder request = new(limiters);
            bool result = request.ValidateClientToken(string.Empty);

            Assert.IsFalse(result);
        }


        [Test, Order(5)]
        public void Check_LastCallRule_EmptyInput()
        {
            List<IRateLimiter> limiters = new();
            limiters.Add(new LastCallRule());

            RuleValidatorBuilder request = new(limiters);
            bool result = request.ValidateClientToken(string.Empty);

            Assert.IsFalse(result);
        }

        [Test, Order(6)]
        public void Check_LastCallRule_OK()
        {
            List<IRateLimiter> limiters = new();
            limiters.Add(new LastCallRule());

            RuleValidatorBuilder request = new(limiters);
            bool result = request.ValidateClientToken("abc");

            Assert.IsTrue(result);
        }

        [Test, Order(7)]
        public void Check_LastCallRule_NOTOK_2MIN()
        {
            List<IRateLimiter> limiters = new();
            limiters.Add(new RequestRateRule());

            RuleValidatorBuilder request = new(limiters);
            bool result = false;
            for (int i = 0; i < 10; i++)
                result = request.ValidateClientToken("abc");

            Thread.Sleep(120000); // Pause for 2 min

            for (int i = 0; i < 2; i++)
                result = request.ValidateClientToken("abc");


            Assert.IsFalse(result);
        }

        [Test, Order(7)]
        public void Check_LastCallRule_OK_1MIN()
        {
            List<IRateLimiter> limiters = new();
            limiters.Add(new RequestRateRule());

            RuleValidatorBuilder request = new(limiters);
            bool result = false;
            for (int i = 0; i < 5; i++)
                result = request.ValidateClientToken("abc");

            Thread.Sleep(60000); // Pause for 1 min

            result = request.ValidateClientToken("abc");


            Assert.IsTrue(result);
        }

        [Test, Order(7)]
        public void Check_LastCallRule_NOTOK_10000Requests()
        {
            List<IRateLimiter> limiters = new();
            limiters.Add(new RequestRateRule());

            RuleValidatorBuilder request = new(limiters);
            bool result = false;
            for (int i = 0; i < 10000; i++)
            {
                Thread.Sleep(1000);
                result = request.ValidateClientToken("abc");
            }

            Assert.IsFalse(result);
        }
    }
}
