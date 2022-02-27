using NUnit.Framework;
using RateLimiter.TokenMatchers;

namespace RateLimiter.Tests.TokenMatchers
{
	public class AllMatcherTests
	{
		[TestFixture]
		public class MatchesToken
		{
			[Test]
			public void AlwaysMatches()
			{
				var token = new AccessToken
				{
					UserId = 1,
					CountryCode = "US",
					IPAddress = "127.0.0.1"
				};

				var matcher = new AllMatcher<int>();

				var result = matcher.MatchesToken(token);

				Assert.IsTrue(result);
			}
		}
	}
}
