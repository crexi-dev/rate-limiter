namespace RateLimiter.Rules.Extensions
{
    public static class AccessRuleExtensions
    {
        public static AccessRule Or(this AccessRule firstAccessRule, AccessRule secondAccessRule)
        {
            return new OrAccessRule(firstAccessRule, secondAccessRule);
        }

        public static AccessRule And(this AccessRule firstAccessRule, AccessRule secondAccessRule)
        {
            return new AndAccessRule(firstAccessRule, secondAccessRule);
        }
    }
}