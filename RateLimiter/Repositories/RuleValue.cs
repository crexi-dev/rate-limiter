using RateLimiter.RuleTemplates;

namespace RateLimiter.Repositories;

public record RuleValue(IRuleTemplate Template, RuleTemplateParams Params);
