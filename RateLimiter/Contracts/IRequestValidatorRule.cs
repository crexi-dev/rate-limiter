using System;

namespace RateLimiter;

public interface IRequestValidatorRule
{
    public bool ValidateRequest(UserSettings user, RuleSettings settings, ValidationRequest request);
}