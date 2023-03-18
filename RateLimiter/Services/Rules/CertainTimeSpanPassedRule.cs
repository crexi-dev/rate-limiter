using System.Collections.Concurrent;
using RateLimiter.Services.Rules.Models;

namespace RateLimiter.Services.Rules;

internal class CertainTimeSpanPassedRule : IRule
{
    private const int CertainTimeInSeconds = 10;
    private readonly ConcurrentDictionary<string, DateTime> _lastLogins = new();
    
    public bool IsAllowed(ClientInfo clientInfo)
    {
        var clientId = clientInfo.ClientId;

        if (_lastLogins.ContainsKey(clientId)
            && _lastLogins[clientId] > DateTime.UtcNow - TimeSpan.FromSeconds(CertainTimeInSeconds))
        {
            return false;
        }

        _lastLogins[clientId] = DateTime.UtcNow;
        return true;
    }
}