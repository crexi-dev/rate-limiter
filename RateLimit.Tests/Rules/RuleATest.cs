using FluentAssertions;
using RateLimit.Contracts;
using RateLimit.Restictions;
using RateLimit.Rules;
using Xunit;

namespace RateLimit.Tests.Rules
{
	public class RuleATest
	{
		[Fact]
		public async void RuleA_CorrectRestrictionAssigned_Test()
		{
			var restriction = new XRequestPerDatespanRestictions(null, null);

			var rule = Record.Exception(() => new RuleA(new List<IRestriction> { restriction }));

			rule.Should().BeNull();
		}

		[Fact]
		public async void RuleA_InCorrectRestictionAssigned_Test()
		{
			var restriction = new TimeSpanPassedSinceLastCallRestictions(null, null);

			Assert.Throws<InvalidOperationException>(() => new RuleA(new List<IRestriction> { restriction }));
		}
	}
}