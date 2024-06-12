using System.Threading.Tasks;

namespace RateLimiter.RateLimiters;

public sealed class RulesBasedRateLimiter : IRateLimiter
{
	private readonly IRulesStorage _rulesStorage;
	private readonly IClientsStorage _clientsStorage;

	public RulesBasedRateLimiter(IRulesStorage rulesStorage, IClientsStorage clientsStorage)
	{
		_rulesStorage = rulesStorage;
		_clientsStorage = clientsStorage;
	}

	public async Task<bool> AllowRequest(string resource, string token)
	{
		(var success, var client) = await _clientsStorage.GetClient(token);
		if (!success) return false;

		var rule = await _rulesStorage.GetRule(resource);

		return rule.Allows(client);
	}
}
