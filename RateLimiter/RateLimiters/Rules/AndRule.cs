namespace RateLimiter.RateLimiters.Rules;

public sealed class AndRule : IRule
{
	private readonly IRule[] _rules;

	public AndRule(IRule[] rules)
	{
		_rules = rules;
	}

	public bool Allows(Client client)
	{
		foreach (var rule in _rules)
		{
			if (!rule.Allows(client)) return false;
		}

		return true;
	}
}
