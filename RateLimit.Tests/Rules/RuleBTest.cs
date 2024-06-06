using FluentAssertions;
using RateLimit.Contracts;
using RateLimit.DTO;
using RateLimit.Restictions;
using RateLimit.Rules;
using Xunit;

namespace RateLimit.Tests.Rules
{
	public class RuleBTest
	{
		
		[Fact]
		public async void RuleB_CorrectRestrictionAssigned_Test()
		{
			var restriction = new TimeSpanPassedSinceLastCallRestictions(null, null);

			var rule = Record.Exception(() => new RuleB(new List<IRestriction> { restriction }));

			rule.Should().BeNull();
		}

		[Fact]
		public async void RuleB_InCorrectRestictionAssigned_Test()
		{
			var restriction = new XRequestPerDatespanRestictions(null, null);

			Assert.Throws<InvalidOperationException>(() => new RuleB(new List<IRestriction> { restriction }));
		}
	}
}