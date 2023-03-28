using System;

namespace RateLimiter.Rules;

public class TimeSpanPassedSinceLastCallRule : RateLimitRule
{
    private readonly Guid? _clientIdentifier;
    private readonly Configuration _configuration;

    public TimeSpanPassedSinceLastCallRule(Guid? clientIdentifier, Configuration configuration)
    {
        _clientIdentifier = clientIdentifier;
        _configuration = configuration;
    }

    public override bool Handle(Request request)
    {
        var requestLastCallDateTime = RequestsBucket.LastCallDateTime(_clientIdentifier);

        if (!requestLastCallDateTime.HasValue)
        {
            return base.Handle(request);
        }

        var timePassedSinceLastCall = (request.CreateTime - requestLastCallDateTime);

        if (timePassedSinceLastCall > _configuration.TimeSpan && RequestsBucket.Count(_clientIdentifier) >= _configuration.Limit)
        {
            return false;
        }
        else
        {
            return base.Handle(request);
        }
    }
}