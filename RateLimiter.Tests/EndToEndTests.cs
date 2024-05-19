using RateLimiter.Models;
using RateLimiter.RuleTemplates;
using RateLimiter.RuleTemplates.Params;
using RateLimiter.Tests.Helpers;
using System;
using Unity;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class EndToEndTests
    {
        private IUnityContainer _unity = new UnityContainer();
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _unity.RegisterDependencies();
        }

        [Test]
        public void AddSingleRule_ApplyToRequests()
        {
            var builder = _unity.Resolve<RuleBuilder>();
            var clientId = Guid.NewGuid();
            var timestamp = new DateTime(2001, 1, 1, 1, 1, 0);

            builder.Add("1", 
                clientId, 
                new RequestByTimeSpanRuleTemplate(), 
                new RequestByTimeSpanRuleTemplateParams { RequestLimit = 1, TimeSpanInSeconds = 100});


            var validator = _unity.Resolve<RequestValidator>();
            bool result = 
                validator.Validate(new Request{ 
                    Resource = "1", 
                    Timestamp = timestamp, 
                    Token = new () { ClientId = clientId } });
            

            result.ShouldBeTrue();

            result = validator.Validate(new Request
            {
                Resource = "1",
                Timestamp = timestamp.AddSeconds(10),
                Token = new() { ClientId = clientId }
            });

            result.ShouldBeFalse();
        }
    }
}
