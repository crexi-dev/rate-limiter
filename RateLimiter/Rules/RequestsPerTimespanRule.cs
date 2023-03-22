using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.Interfaces;
using RateLimiter.Services;

namespace RateLimiter.Rules;

public class RequestsPerTimespanRule : IRateLimitingRule {
    private readonly int _maxRequests;
    private readonly TimeSpan _timeSpan;
    private readonly RequestCountDatabase _database;

    public RequestsPerTimespanRule(RequestCountDatabase database, int maxRequests, TimeSpan timeSpan) {
        _maxRequests = maxRequests;
        _timeSpan = timeSpan;
        _database = database;
    }

    public bool IsAllowed(string clientIdentifier, string resourceAccessed) {
        if (String.IsNullOrEmpty(clientIdentifier) || String.IsNullOrEmpty(resourceAccessed)) {
            return false;
        }

        var requests = _database.Get(clientIdentifier);

        if (requests == null) {
            return true;
        }

        Console.WriteLine("request.ResourceAccessed");

        var count = 0;
        var now = DateTime.Now;

        foreach (var request in requests) {
            Console.WriteLine(request.ResourceAccessed);
            if (request.DateOfAccess.Add(_timeSpan) > now && request.ResourceAccessed.Equals(resourceAccessed)) {
                count++;
            }
        }

        return count < _maxRequests;
    }


    public void LogRequest(string clientIdentifier, IClientRequest requestToLog) {
        var oldRequests = _database.Get(clientIdentifier);
        if (oldRequests == null) {
            _database.Add(clientIdentifier, new[] { requestToLog });
        } else {
            var requests = oldRequests.Concat(new[] { requestToLog });
            _database.Update(clientIdentifier, requests, oldRequests);
        }
    }
}