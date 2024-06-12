namespace RateLimiter.RateLimiters;

public sealed class Client
{
	public string Region { get; }

	public Client(string region)
	{
		Region = region;
	}
}
