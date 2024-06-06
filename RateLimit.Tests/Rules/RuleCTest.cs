using FluentAssertions;
using RateLimit.Contracts;
using RateLimit.DTO;
using RateLimit.Restictions;
using RateLimit.Rules;
using Xunit;

namespace RateLimit.Tests.Rules
{
	public class RuleCTest
	{
		

		[Fact]
		public async void RuleC_CorrectRestrictionAssigned_Test()
		{
			var restriction = new TimeSpanPassedSinceLastCallRestictions(null, null);
			var restriction2 = new XRequestPerDatespanRestictions(null, null);

			var rule = Record.Exception(() => new RuleB(new List<IRestriction> { restriction, restriction2 }));

			rule.Should().BeNull();
		}


		public static IEnumerable<object[]> RuleC_InCorrectRestictionAssigned_Data()
		{
			yield return new object[]
			{
				new List<IRestriction>{}
			};
			yield return new object[]
			{
				new List<IRestriction>{new XRequestPerDatespanRestictions(null, null)}
			};
			yield return new object[]
			{
				new List<IRestriction>{new TimeSpanPassedSinceLastCallRestictions(null, null)}
			};
		}

		[Theory]
		[MemberData(nameof(RuleC_InCorrectRestictionAssigned_Data))]
		public async void RuleC_InCorrectRestictionAssigned_Test(List<IRestriction> restrictions)
		{
			Assert.Throws<InvalidOperationException>(() => new RuleC(restrictions));
		}
	}
}