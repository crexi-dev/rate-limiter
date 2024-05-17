using System;

namespace RateLimiter.Repositories;

internal record RuleKey(string Resource, Guid ClientId);
