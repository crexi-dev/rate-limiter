using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.Interfaces;
using RateLimiter.Models;

namespace RateLimiter.Services;

public class Limiter {
    private readonly List<IRateLimitingRule> _rules;

    public Limiter() {
        _rules = new List<IRateLimitingRule>();
    }

    public void AddRule(IRateLimitingRule ruleToAdd) {
        _rules.Add(ruleToAdd);
    }

    public bool isRequestAllowed(string clientIdentifier, string resourceAccessed) {
        _rules.All(rule => {
            Console.WriteLine(rule);
            return rule.IsAllowed(clientIdentifier, resourceAccessed);
        });
        return _rules.Count == 0 || _rules.All(rule => rule.IsAllowed(clientIdentifier, resourceAccessed));
    }

    public void LogRequest(string clientIdentifier, ClientRequest requestToLog) {
        foreach (var rule in _rules) {
            Console.WriteLine(requestToLog.ResourceAccessed);
            rule.LogRequest(clientIdentifier, requestToLog);
        }
    }
}