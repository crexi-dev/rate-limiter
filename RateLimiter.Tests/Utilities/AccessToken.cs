namespace RateLimiter.Tests.Utilities
{
	internal record AccessToken(int UserId, string IPAddress, string CountryCode) : IAccessToken<int>;
}
