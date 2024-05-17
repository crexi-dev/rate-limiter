using RateLimiter.RuleTemplates;

namespace RateLimiter.Repositories;

internal record RuleValue(IRuleTemplate Template, RuleTemplateParams Params);
