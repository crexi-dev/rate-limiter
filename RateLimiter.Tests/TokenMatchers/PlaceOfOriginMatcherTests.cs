using NUnit.Framework;
using RateLimiter.TokenMatchers;

namespace RateLimiter.Tests.TokenMatchers
{
	public class PlaceOfOriginMatcherTests
	{
		[TestFixture]
		public class MatchesToken
		{
			[TestCase("")]
			[TestCase("US")]
			public void EmptyCountryCodesNeverMatches(string placeOfOrigin)
			{
				var accessToken = new AccessToken
				{
					UserId = 1,
					IPAddress = "127.0.0.1",
					CountryCode = placeOfOrigin
				};

				var matcher = new PlaceOfOriginMatcher<int>();

				var result = matcher.MatchesToken(accessToken);

				Assert.False(result);
			}

			[Test]
			public void EmptyInputNeverMatches()
			{
				var accessToken = new AccessToken
				{
					UserId = 1,
					IPAddress = "127.0.0.1",
					CountryCode = string.Empty
				};

				var matcher = new PlaceOfOriginMatcher<int>("US", "CA", "MX");

				var result = matcher.MatchesToken(accessToken);

				Assert.False(result);
			}

			[Test]
			public void MissingCountryCodeDoesNotMatch()
			{
				var accessToken = new AccessToken
				{
					UserId = 1,
					IPAddress = "127.0.0.1",
					CountryCode = "ES"
				};

				var matcher = new PlaceOfOriginMatcher<int>("DE", "FR");

				var result = matcher.MatchesToken(accessToken);

				Assert.False(result);
			}

			[Test]
			public void UpperCaseMatches()
			{
				var countryCode = "US";

				var accessToken = new AccessToken
				{
					UserId = 1,
					IPAddress = "127.0.0.1",
					CountryCode = countryCode
				};

				var matcher = new PlaceOfOriginMatcher<int>(countryCode);

				var result = matcher.MatchesToken(accessToken);

				Assert.True(result);
			}

			[Test]
			public void LowerCaseMatches()
			{
				var countryCode = "ca";

				var accessToken = new AccessToken
				{
					UserId = 1,
					IPAddress = "127.0.0.1",
					CountryCode = countryCode
				};

				var matcher = new PlaceOfOriginMatcher<int>(countryCode);

				var result = matcher.MatchesToken(accessToken);

				Assert.True(result);
			}

			[Test]
			public void DifferentCaseMatches()
			{
				var countryCode = "GB";

				var accessToken = new AccessToken
				{
					UserId = 1,
					IPAddress = "127.0.0.1",
					CountryCode = countryCode
				};

				var matcher = new PlaceOfOriginMatcher<int>("gb");

				var result = matcher.MatchesToken(accessToken);

				Assert.True(result);
			}

			[Test]
			public void CountryCodeAmongListMatches()
			{
				var countryCode = "FR";

				var accessToken = new AccessToken
				{
					UserId = 1,
					IPAddress = "127.0.0.1",
					CountryCode = countryCode
				};

				var matcher = new PlaceOfOriginMatcher<int>("DE", "FR", "BE");

				var result = matcher.MatchesToken(accessToken);

				Assert.True(result);
			}
		}
	}
}
