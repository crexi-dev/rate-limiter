using NUnit.Framework;
using RateLimiter.Tests.Utilities;
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
				var token = new AccessToken(1, "127.0.0.1", "US");

				var matcher = new AllMatcher<int>();

				var result = matcher.MatchesToken(token);

				Assert.IsTrue(result);
			}
		}
	}
}
