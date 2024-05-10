using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using RateLimiter.Executors;
using RateLimiter.Models;
using RateLimiter.Processor;
using RateLimiter.Reader;
using System;
using System.Collections.Generic;

namespace RateLimiter.Tests.Processor
{
    [TestFixture]
    public class RateLimitProcessorTests
    {
        [TestCase]
        public void RateLimitProcessor_Throws_ArgumentNullException_Rules()
        {
            var httpRequest = new Mock<HttpRequest>();
            httpRequest.Setup(x => x.Method).Returns(It.IsAny<string>);
            httpRequest.Setup(x => x.Path).Returns(It.IsAny<PathString>);
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.Request)
                .Returns(httpRequest.Object);
            var reader = new Mock<IRuleReader>();
            reader.Setup(x => x.ReadRules(It.IsAny<ReadRulesRequestModel>()))
            .Returns((IEnumerable<ReadRuleResponseModel>)null);

            var rateLimitProcessor = new RateLimitProcessor(reader.Object, new Mock<IRuleExecutor>().Object, new Mock<IMapper>().Object);

            Assert.Throws<ArgumentNullException>(() => rateLimitProcessor.Process(httpContext.Object));
        }

        [TestCase]
        public void RateLimitProcessor_Empty_Rules_Returns_True()
        {
            var httpRequest = new Mock<HttpRequest>();
            httpRequest.Setup(x => x.Method).Returns(It.IsAny<string>);
            httpRequest.Setup(x => x.Path).Returns(It.IsAny<PathString>);
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.Request)
                .Returns(httpRequest.Object);
            var reader = new Mock<IRuleReader>();
            reader.Setup(x => x.ReadRules(It.IsAny<ReadRulesRequestModel>()))
            .Returns(new List<ReadRuleResponseModel> { });

            var rateLimitProcessor = new RateLimitProcessor(reader.Object, new Mock<IRuleExecutor>().Object, new Mock<IMapper>().Object);

            var response = rateLimitProcessor.Process(httpContext.Object);

            Assert.IsTrue(response);
        }

        [TestCase]
        public void RateLimitProcessor_Throws_ArgumentNullException_Token()
        {
            var httpRequest = new Mock<HttpRequest>();
            httpRequest.Setup(x => x.Method).Returns(It.IsAny<string>);
            httpRequest.Setup(x => x.Path).Returns(It.IsAny<PathString>);
            httpRequest.Setup(x => x.Headers["Authorization"]).Returns(string.Empty);
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.Request)
                .Returns(httpRequest.Object);
            var rulesResponse = new List<ReadRuleResponseModel> { new ReadRuleResponseModel { } };
            var reader = new Mock<IRuleReader>();
            reader.Setup(x => x.ReadRules(It.IsAny<ReadRulesRequestModel>()))
            .Returns(rulesResponse); 
            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<IEnumerable<RuleExecuteRequestModel>>(rulesResponse))
                .Returns((IEnumerable<RuleExecuteRequestModel>)null);

            var rateLimitProcessor = new RateLimitProcessor(reader.Object, new Mock<IRuleExecutor>().Object, mapper.Object);

            Assert.Throws<ArgumentNullException>(() => rateLimitProcessor.Process(httpContext.Object));
        }

        [TestCase("test", true)]
        [TestCase("test1", true)]
        [TestCase("test", false)]
        [TestCase("test", false)]
        public void RateLimitProcessor_Success(string token, bool response)
        {
            var httpRequest = new Mock<HttpRequest>();
            httpRequest.Setup(x => x.Method).Returns(It.IsAny<string>);
            httpRequest.Setup(x => x.Path).Returns(It.IsAny<PathString>);
            httpRequest.Setup(x => x.Headers["Authorization"]).Returns(token);
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.Request)
                .Returns(httpRequest.Object);
            var rulesResponse = new List<ReadRuleResponseModel> { new ReadRuleResponseModel { } };
            var reader = new Mock<IRuleReader>();
            reader.Setup(x => x.ReadRules(It.IsAny<ReadRulesRequestModel>()))
            .Returns(rulesResponse);
            var mapperReseponse = new List<RuleExecuteRequestModel> { };
            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<IEnumerable<RuleExecuteRequestModel>>(rulesResponse))
                .Returns(mapperReseponse);

            var executor = new Mock<IRuleExecutor>();
            executor.Setup(x => x.ExecuteRules(mapperReseponse, token))
            .Returns(response);

            var rateLimitProcessor = new RateLimitProcessor(reader.Object, executor.Object, mapper.Object);

            Assert.That(rateLimitProcessor.Process(httpContext.Object), Is.EqualTo(response));
        }
    }
}
