using RateLimiter.Services.Rules;
using RateLimiter.Services.Rules.Models;

namespace RateLimiter.Services;

internal class RulesService : IRulesService
{
    private readonly IRule _certainTimeSpanPassedRule = new CertainTimeSpanPassedRule();
    private readonly IRule _xRequestsPerTimeSpanRule = new XRequestsPerTimeSpanRule();
    private readonly IRule _defaultPassedRule;

    public RulesService()
    {
        _defaultPassedRule = _certainTimeSpanPassedRule;
    }

    public IRule GetRule(ClientInfo clientInfo)
    {
        if (clientInfo.AccessToken != null)
            return GetRuleByAccessToken(clientInfo.AccessToken);

        if (clientInfo.Ip != null)
            return GetRuleByIp(clientInfo.Ip);

        return _defaultPassedRule;
    }

    private IRule GetRuleByAccessToken(string accessToken)
    {
        if(accessToken == "TypeA")
            return _certainTimeSpanPassedRule;

        if (accessToken == "TypeB")
            return _xRequestsPerTimeSpanRule;

        return _defaultPassedRule;
    }

    private IRule GetRuleByIp(string ip)
    {
        return _defaultPassedRule;
    }
}