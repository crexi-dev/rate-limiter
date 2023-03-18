using RateLimiter.Services.Rules.Models;
using System.Collections.Concurrent;

namespace RateLimiter.Services.Rules;

internal class XRequestsPerTimeSpanRule : IRule
{
    private const int RequestsCount = 3;
    private const int CertainTimeInSeconds = 30;
    private readonly ConcurrentDictionary<string, int> _loginCountList = new();
    private Timer _timer;

    public XRequestsPerTimeSpanRule()
    {
        _timer = new Timer(Count, null, 0, CertainTimeInSeconds * 1000);
    }

    public bool IsAllowed(ClientInfo clientInfo)
    {
        var clientId = clientInfo.ClientId;

        if (!_loginCountList.ContainsKey(clientId))
        {
            _loginCountList[clientId] = 1;
            return true;
        }
        if (_loginCountList[clientId] < RequestsCount)
        {
            _loginCountList[clientId]++;
            return true;
        }

        return false;
    }

    public void Count(object? obj)
    {
        foreach (var key in _loginCountList.Keys)
        {
            _loginCountList[key] = 0;
        }
    }
}