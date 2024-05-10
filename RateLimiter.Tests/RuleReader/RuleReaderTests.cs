using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RateLimiter.Models;
using RateLimiter.Reader;
using System.Collections.Generic;

namespace RateLimiter.Tests.RuleReader
{
    [TestFixture]
    public class RuleReaderTests
    {
        [TestCase]
        public void RuleReader_Success()
        {
            var confgModel = new List<RuleConfigurationModel>();
            var option = new Mock<IOptions<List<RuleConfigurationModel>>>();
            option.Setup(x => x.Value).Returns(confgModel);
            var result = new List<ReadRuleResponseModel>();
            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<IEnumerable<ReadRuleResponseModel>>(confgModel))
                .Returns(result);

            var reader = new RateLimiter.Reader.RuleReader(option.Object, mapper.Object);

            var response = reader.ReadRules(new ReadRulesRequestModel { RequestAction = It.IsAny<string>(), RequestPath = It.IsAny<string>() });

            Assert.That(response, Is.EqualTo(result));
        }
    }
}
