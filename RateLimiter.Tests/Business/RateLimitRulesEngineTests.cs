using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RateLimiter.Business;
using RateLimiter.Configuration;
using RateLimiter.Interfaces.Business.Rules;
using RateLimiter.Interfaces.Configuration;
using RateLimiter.Interfaces.Factories;
using RateLimiter.Interfaces.Models;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace RateLimiter.Tests.Business
{
    [TestClass]
    public class RateLimitRulesEngineTests
    {
        private Mock<IRateLimitConfiguration> _configuration;
        private Mock<ILogger<RateLimitRulesEngine>> _logger;
        private Mock<IRateLimitRuleFactory> _rateLimitRuleFactory;

        [TestInitialize]
        public void Initialize()
        {
            _configuration = new Mock<IRateLimitConfiguration>();
            _logger = new Mock<ILogger<RateLimitRulesEngine>>();
            _rateLimitRuleFactory = new Mock<IRateLimitRuleFactory>();
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        [DataRow(true, false, false)]
        [DataRow(false, true, false)]
        [DataRow(false, false, true)]
        public void InvalidConstructorTest(bool isConfigurationNull, bool isLoggerNull, bool isRuleFactoryNull)
        {
            var configuration = isConfigurationNull ? null : _configuration.Object;
            var logger = isLoggerNull ? null : _logger.Object;
            var ruleFactory = isRuleFactoryNull ? null : _rateLimitRuleFactory.Object;

            new RateLimitRulesEngine(configuration, logger, ruleFactory);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void BadArgumentAddRulesFactoryTest()
        {
            var target = new RateLimitRulesEngine(_configuration.Object, _logger.Object, _rateLimitRuleFactory.Object);

            target.AddRulesFactory(null);
        }

        [TestMethod]
        public void AddRulesFactoryTest()
        {
            var target = new RateLimitRulesEngine(_configuration.Object, _logger.Object, _rateLimitRuleFactory.Object);

            var accessor = new PrivateObject(target);

            var rulesFactory = new Mock<IRateLimitRuleFactory>();

            target.AddRulesFactory(rulesFactory.Object);

            var factories = (List<IRateLimitRuleFactory>)accessor.GetField("_rateLimitRuleFactories");

            Assert.AreSame(rulesFactory.Object, factories[1]);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public async Task RunRuleWithoutFactoryTest()
        {
            var user = new User(Guid.NewGuid().ToString());

            var endpoint = CreateEndpoint(".*", HttpVerbFlags.Get, "NoFactoryForThisRuleId");

            var endpoints = new List<Endpoint>
            {
                endpoint
            };

            _configuration.Setup(x => x.Endpoints).Returns(endpoints.ToArray());

            var request = CreateRequest("get", "/123");

            var context = new Mock<HttpContext>();

            context.Setup(x => x.Request).Returns(request.Object);

            var target = new RateLimitRulesEngine(_configuration.Object, _logger.Object, _rateLimitRuleFactory.Object);

            await target.Run(context.Object, user);
        }

        [TestMethod]
        public async Task UnsupportedVerbRunTest()
        {
            var user = new User(Guid.NewGuid().ToString());

            var request = CreateRequest("trace", "/WeatherForecast");

            var context = new Mock<HttpContext>();

            context.Setup(x => x.Request).Returns(request.Object);

            var endpoints = new List<Endpoint>();

            _configuration.Setup(x => x.Endpoints).Returns(endpoints.ToArray());

            var target = new RateLimitRulesEngine(_configuration.Object, _logger.Object, _rateLimitRuleFactory.Object);

            var actual = await target.Run(context.Object, user);

            Assert.AreEqual(true, actual.Proceed);
        }

        [TestMethod]
        public async Task NoEndpointsRunTest()
        {
            var request = CreateRequest("get", "/WeatherForecast");

            var endpoints = new List<Endpoint>();

            _configuration.Setup(x => x.Endpoints).Returns(endpoints.ToArray());

            var context = new Mock<HttpContext>();

            context.Setup(x => x.Request).Returns(request.Object);

            var user = new User(Guid.NewGuid().ToString());

            var target = new RateLimitRulesEngine(_configuration.Object, _logger.Object, _rateLimitRuleFactory.Object);

            var actual = await target.Run(context.Object, user);

            Assert.AreEqual(true, actual.Proceed);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task RunTest(bool proceed)
        {
            var user = new User(Guid.NewGuid().ToString());

            var endpoint = CreateEndpoint(".*", HttpVerbFlags.Get, "Rule1");

            var endpoints = new List<Endpoint>
            {
                endpoint
            };

            var request = CreateRequest("get", "/WeatherForecast");

            var result = CreateResult(proceed);

            var context = new Mock<HttpContext>();

            context.Setup(x => x.Request).Returns(request.Object);

            var rule = new Mock<IRateLimitRule>();

            rule.Setup(x => x.Verify(It.IsAny<HttpContext>(), user,
                endpoint, It.IsAny<DateTime>())).Returns(Task.FromResult(result.Object));

            _rateLimitRuleFactory.Setup(x => x.SupportsType(It.IsAny<string>())).Returns(true);
            _rateLimitRuleFactory.Setup(x => x.Create(It.IsAny<IRateLimitRuleConfiguration>())).Returns(rule.Object);

            _configuration.Setup(x => x.Endpoints).Returns(endpoints.ToArray());

            var target = new RateLimitRulesEngine(_configuration.Object, _logger.Object, _rateLimitRuleFactory.Object);

            var actual = await target.Run(context.Object, user);

            Assert.AreEqual(proceed, actual.Proceed);
        }

        private Mock<IRateLimitRuleResult> CreateResult(bool proceed)
        {
            var mock = new Mock<IRateLimitRuleResult>();

            mock.Setup(x => x.Proceed).Returns(proceed);

            return mock;
        }

        private Mock<HttpRequest> CreateRequest(string verb, string path)
        {
            var mock = new Mock<HttpRequest>();

            mock.Setup(x => x.Method).Returns(verb);
            mock.Setup(x => x.Path).Returns(path);

            return mock;
        }

        private Endpoint CreateEndpoint(string pattern, HttpVerbFlags verbs, string ruleName)
        {
            return new Endpoint
            {
                PathPattern = pattern,
                Verbs = verbs,
                Rules = [
                    new RateLimitRuleConfiguration {
                        Type = ruleName,
                        Parameters = new Dictionary<string, object>(){
                        }
                    } ]
            };
        }
    }
}
