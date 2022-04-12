using NUnit.Framework;
using RateLimiter.Interfaces;
using RateLimiter.Repository;
using RateLimiter.Rules;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task Check_LastCallRule_NOTOK_2MINAsync()
        {
            List<IRateLimiter> limiters = new();
            limiters.Add(new LastCallRule());

            RuleValidatorBuilder request = new(limiters);
            bool result = false;
            for (int i = 0; i < 10; i++)
                result = request.ValidateClientToken("abc");

            await Task.Delay(120000); // Pause for 2 min

            for (int i = 0; i < 2; i++)
                result = request.ValidateClientToken("abc");


            Assert.IsFalse(result);
        }

        [Test, Order(8)]
        public async Task Check_LastCallRule_NOTOK_1MINAsync()
        {
            List<IRateLimiter> limiters = new();
            limiters.Add(new LastCallRule());

            RuleValidatorBuilder request = new(limiters);
            bool result = false;
            for (int i = 0; i < 5; i++)
                result = request.ValidateClientToken("abc");

            await Task.Delay(60000); // Pause for 1 min

            result = request.ValidateClientToken("abc");


            Assert.IsFalse(result);
        }

        [Ignore("Need to check")]
        [Test, Order(9)]
        public async Task Check_LastCallRule_NOTOK_15RequestsAsync()
        {
            List<IRateLimiter> limiters = new();
            limiters.Add(new LastCallRule());

            RuleValidatorBuilder request = new(limiters);
            bool result = false;
            for (int i = 0; i < 15; i++)
            {
                await Task.Delay(1000);
                result = request.ValidateClientToken("abc");
            }

            Assert.IsFalse(result);
        }


        [Test, Order(10)]
        public void Check_Apply_RequestRateRule_LastCallRule_NOTOK()
        {
            List<IRateLimiter> limiters = new();
            limiters.Add(new RequestRateRule()); // max request limit is 5
            limiters.Add(new LastCallRule()); // 1 min is max time passed since last call

            // This test failed as the request rate failed (Max limit is 5 and requests sent is 100)

            RuleValidatorBuilder request = new(limiters);
            bool result = false;
            for (int i = 0; i < 100; i++)
            {
                result = request.ValidateClientToken("abc");
            }
           
            Assert.IsFalse(result);
        }

        [Test, Order(11)]
        public void Check_Apply_RequestRateRule_LastCallRule_OK()
        {
            List<IRateLimiter> limiters = new();
            limiters.Add(new RequestRateRule()); // max request limit is 5
            limiters.Add(new LastCallRule()); // 1 min is max time passed since last call

            RuleValidatorBuilder request = new(limiters);
            bool result = false;
            for (int i = 0; i < 5; i++)
            {
                result = request.ValidateClientToken("abc");
            }

            Assert.IsTrue(result);
        }

        [Test, Order(12)]
        public async Task Check_Apply_RequestRateRule_LastCallRule_Failed_LastCallRule_NOTOKAsync()
        {
            List<IRateLimiter> limiters = new();
            limiters.Add(new RequestRateRule()); // max request limit is 5
            limiters.Add(new LastCallRule()); // 1 min is max time passed since last call

            RuleValidatorBuilder request = new(limiters);
            bool result = false;
            for (int i = 0; i < 5; i++)
                result = request.ValidateClientToken("abc");
            

            await Task.Delay(60000); // Pause for 1 min

            result = request.ValidateClientToken("abc");

            Assert.IsFalse(result);
        }
    }
}
