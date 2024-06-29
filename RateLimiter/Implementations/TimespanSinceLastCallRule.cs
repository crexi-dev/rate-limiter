using System.Collections.Generic;
using System;

public class TimespanSinceLastCallRule : IRateLimitRule
{
    private readonly TimeSpan _minTimespan;
    private readonly Dictionary<(string clientId, string resourceId), DateTime> _lastCallLog;

    public TimespanSinceLastCallRule(TimeSpan minTimespan)
    {
        _minTimespan = minTimespan;
        _lastCallLog = new Dictionary<(string, string), DateTime>();
    }

    public bool IsRequestAllowed(string clientId, string resourceId)
    {
        var now = DateTime.Now;
        var key = (clientId, resourceId);

        if (!_lastCallLog.ContainsKey(key) || now - _lastCallLog[key] >= _minTimespan)
        {
            return true;
        }

        return false;
    }

    public void RecordRequest(string clientId, string resourceId)
    {
        var now = DateTime.Now;
        var key = (clientId, resourceId);

        _lastCallLog[key] = now;
    }
}