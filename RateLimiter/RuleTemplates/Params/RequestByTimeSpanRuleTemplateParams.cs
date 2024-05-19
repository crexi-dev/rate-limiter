namespace RateLimiter.RuleTemplates.Params;

public class RequestByTimeSpanRuleTemplateParams : RuleTemplateParams
{
    public int RequestLimit { get; internal set; }
    public int TimeSpanInSeconds { get; internal set; }
}
