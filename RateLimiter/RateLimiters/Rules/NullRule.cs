namespace RateLimiter.RateLimiters.Rules;

public sealed class NullRule : IRule
{
	public static IRule Null { get; } = new NullRule();

	private NullRule() { }

	public bool Allows(Client client)
	{
		return false;
	}
}