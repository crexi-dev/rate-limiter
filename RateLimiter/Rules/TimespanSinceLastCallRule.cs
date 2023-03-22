using System;
using RateLimiter.Interfaces;
using RateLimiter.Models;
using RateLimiter.Services;

namespace RateLimiter.Rules;

public class TimespanSinceLastCallRule : IRateLimitingRule {
    private readonly TimeSpan _timeSpan;
    private readonly LatestRequestDatabase _database;

    public TimespanSinceLastCallRule(LatestRequestDatabase database, TimeSpan timeSpan) {
        _timeSpan = timeSpan;
        _database = database;
    }

    public bool IsAllowed(string clientIdentifier, string resourceAccessed) {
        var requestAccessKey = new ClientRequestKey(clientIdentifier, resourceAccessed);
        var lastRequestTime = _database.Get(requestAccessKey);

        if (lastRequestTime == null) {
            return true;
        }

        return DateTime.Now - lastRequestTime > _timeSpan;
    }

    public void LogRequest(string clientIdentifier, IClientRequest requestToLog) {
        var requestAccessKey = new ClientRequestKey(clientIdentifier, requestToLog.ResourceAccessed);
        var oldRequest = _database.Get(requestAccessKey);
        if (oldRequest == null) {
            _database.Add(requestAccessKey, requestToLog.DateOfAccess);
        } else {
            _database.Update(requestAccessKey, requestToLog.DateOfAccess, oldRequest.Value);
        }
    }
}