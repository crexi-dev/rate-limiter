namespace RateLimiter.RateLimiters;

public interface IRule
{
	bool Allows(Client client);
}
