using Moq;
using NUnit.Framework;
using RateLimiter.Rules.RuleInfo;

namespace RateLimiter.Tests.Rules
{
    [TestFixture]
    public class ReqionBasedRuleTests
    {
        private Mock<IRule<FakeRuleInfo>> _innerRule = null!;
        private RegionBasedRule<FakeRuleInfo> _USRegionBasedRule = null!;
        private const string US = "us";
        private const string EU = "eu";

        [SetUp]
        public void SetUp()
        {
            _innerRule = new Mock<IRule<FakeRuleInfo>>();
            _USRegionBasedRule = new RegionBasedRule<FakeRuleInfo>(US, _innerRule.Object);
        }

        [Test]
        public void Validate_SameRegionInnerRuleReturnsTrue_ReturnsTrue()
        {
            // Arrange
            const string region = US;
            var regionBasedInfo = new RegionBasedInfo<FakeRuleInfo> { Region = region, InnerRuleInfo = new FakeRuleInfo() };
            _innerRule.Setup(x => x.Validate(It.IsAny<FakeRuleInfo>())).Returns(true);
            // Act
            
            var result = _USRegionBasedRule.Validate(regionBasedInfo);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Validate_DifferentRegionInnerRuleReturnsFalse_ReturnsTrue()
        {
            // Arrange
            const string region = EU;
            var regionBasedInfo = new RegionBasedInfo<FakeRuleInfo> { Region = region, InnerRuleInfo = new FakeRuleInfo() };
            _innerRule.Setup(x => x.Validate(It.IsAny<FakeRuleInfo>())).Returns(false);

            // Act
            var result = _USRegionBasedRule.Validate(regionBasedInfo);

            // Assert
            Assert.That(result, Is.True);
        }

        public class FakeRuleInfo : Info
        {

        }
    }
}