using System;
using System.Net.NetworkInformation;

namespace RateLimiter.Rules;

public class XRequestPerTimeSpanRule : RateLimitRule
{
    private readonly Guid? _clientIdentifier;
    private readonly Configuration _configuration;

    public XRequestPerTimeSpanRule(Guid? clientIdentifier, Configuration configuration)
    {
        _configuration = configuration;
        _clientIdentifier = clientIdentifier;
    }

    public override bool Handle(Request request)
    {
        var requestFirstCallDateTime = RequestsBucket.FirstCallDateTime(_clientIdentifier);

        if (!requestFirstCallDateTime.HasValue)
        {
            return base.Handle(request);
        }

        var timePassedSinceFirstCall = (request.CreateTime - requestFirstCallDateTime);

        if (timePassedSinceFirstCall > _configuration.TimeSpan && RequestsBucket.Count(_clientIdentifier) >= _configuration.Limit)
        {
            return false;
        }
        else
        {
            return base.Handle(request);
        }
    }
}