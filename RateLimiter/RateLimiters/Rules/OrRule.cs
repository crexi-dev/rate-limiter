namespace RateLimiter.RateLimiters.Rules;

public sealed class OrRule : IRule
{
	private readonly IRule[] _rules;

	public OrRule(params IRule[] rules)
	{
		_rules = rules;
	}

	public bool Allows(Client client)
	{
		foreach (var rule in _rules)
		{
			if (rule.Allows(client)) return true;
		}

		return true;
	}
}