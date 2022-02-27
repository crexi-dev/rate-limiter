using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.TokenMatchers
{
	/// <summary>
	/// A token matcher that matches against the country of origin for an access token.
	/// </summary>
	public class PlaceOfOriginMatcher<T> : ITokenMatcher<T>
	{
		private readonly HashSet<string> _countryCodes;

		public PlaceOfOriginMatcher(params string[] countryCodes)
		{
			_countryCodes = countryCodes
				.Select(countryCode => countryCode.ToUpperInvariant())
				.ToHashSet();
		}

		public bool MatchesToken(IAccessToken<T> accessToken) =>
			_countryCodes.Contains(accessToken.CountryCode.ToUpperInvariant());
	}
}
