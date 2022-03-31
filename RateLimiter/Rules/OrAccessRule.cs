namespace RateLimiter.Rules
{
    public class OrAccessRule: AccessRule
    {
        private readonly AccessRule _firstAccessRule;
        private readonly AccessRule _secondAccessRule;

        public OrAccessRule(AccessRule firstAccessRule, AccessRule secondAccessRule)
        {
            _firstAccessRule = firstAccessRule;
            _secondAccessRule = secondAccessRule;
        }

        public override bool Validate(string resourceName, string accessKey)
        {
            return _firstAccessRule.Validate(resourceName, accessKey) ||
                   _secondAccessRule.Validate(resourceName, accessKey);
        }
    }
}
