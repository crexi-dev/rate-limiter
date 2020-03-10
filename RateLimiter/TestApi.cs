using RateLimiter.Rules;

namespace RateLimiter
{
    public class TestApi
    {
        private readonly ApiRequestValidator _validator = new ApiRequestValidator(new RateLimitRuleFactory());

        public TestApi()
        {

        }

        public string ResourceA(string token)
        {
            return HandleRequest(token, "ResourceA");
        }

        public string ResourceB(string token)
        {
            return HandleRequest(token, "ResourceB");
        }

        public string ResourceC(string token)
        {
            return HandleRequest(token, "ResourceC");
        }

        private string HandleRequest(string token, string resourceName)
        {
            if (_validator.ValidateRequest(token, resourceName))
                return "Call succeeded";

            return "Call rate limited";
        }
    }
}
