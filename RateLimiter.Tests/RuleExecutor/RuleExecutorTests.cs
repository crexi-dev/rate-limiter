using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using RateLimiter.Executors;
using RateLimiter.Models;
using System;

namespace RateLimiter.Tests.RuleExecutor
{
    [TestFixture]
    public class RuleExecutorTests
    {
        [TestCase("a")]
        [TestCase("b")]
        [TestCase("c")]
        public void RuleExecutor_Execute_Type_Is_Not_Found(string name)
        {
            var executor = new RateLimiter.Executors.RuleExecutor(new Mock<IServiceProvider>().Object, new Mock<IMapper>().Object);
            Assert.Throws<ArgumentNullException>(() => executor.ExecuteRule(new Models.RuleExecuteRequestModel
            {
                Name = name,

            }, It.IsAny<string>()));
        }

        [TestCase("RequestIntervalRule")]
        [TestCase("LimitPeriodRule")]
        public void RuleExecutor_Execute_Fail(string name)
        {
            var model = new Models.RuleExecuteRequestModel
            {
                Name = name
            };
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x => x.GetService(typeof(IMemoryCache)))
                .Returns(new Mock<IMemoryCache>().Object);
            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<RateLimitRuleModel>(model))
                .Returns(new RateLimitRuleModel { });
            var executor = new RateLimiter.Executors.RuleExecutor(serviceProvider.Object, mapper.Object);
            var response = executor.ExecuteRule(model, It.IsAny<string>());

            Assert.IsFalse(response);
        }
    }
}
