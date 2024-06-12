namespace RateLimiter.RateLimiters.Rules;

public sealed class RegionBaseRule : IRule
{
	private readonly IRule _usRule;
	private readonly IRule _euRule;

	public RegionBaseRule(IRule usRule, IRule euRule)
	{
		_usRule = usRule;
		_euRule = euRule;
	}

	public bool Allows(Client client)
	{
		if (client.Region == "US") // constant here just for an example
		{
			return _usRule.Allows(client);
		}
		else if (client.Region == "EU")
		{
			return _euRule.Allows(client);
		}

		return false;
	}
}
