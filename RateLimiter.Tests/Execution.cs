using System;

namespace RateLimiter
{
    public record Execution(string Token, string Description, DateTime CompletedOn);
}