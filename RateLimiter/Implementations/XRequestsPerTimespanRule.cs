using System;
using System.Collections.Generic;
using System.Linq;

public class XRequestsPerTimespanRule : IRateLimitRule
{
    private readonly int _maxRequests;
    private readonly TimeSpan _timespan;
    private readonly Dictionary<(string clientId, string resourceId), List<DateTime>> _requestLog;

    public XRequestsPerTimespanRule(int maxRequests, TimeSpan timespan)
    {
        _maxRequests = maxRequests;
        _timespan = timespan;
        _requestLog = new Dictionary<(string, string), List<DateTime>>();
    }

    public bool IsRequestAllowed(string clientId, string resourceId)
    {
        var now = DateTime.Now;
        var key = (clientId, resourceId);

        if (!_requestLog.ContainsKey(key))
        {
            _requestLog[key] = new List<DateTime>();
        }

        _requestLog[key] = _requestLog[key].Where(time => now - time <= _timespan).ToList();

        return _requestLog[key].Count < _maxRequests;
    }

    public void RecordRequest(string clientId, string resourceId)
    {
        var now = DateTime.Now;
        var key = (clientId, resourceId);

        if (!_requestLog.ContainsKey(key))
        {
            _requestLog[key] = new List<DateTime>();
        }

        _requestLog[key].Add(now);
    }
}

