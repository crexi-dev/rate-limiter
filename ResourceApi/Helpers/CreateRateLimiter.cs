using RateLimiter.Interfaces;
using RateLimiter.Limiter;
using RateLimiter.Rules;
using ResourceApi.Controllers;

namespace ResourceApi.Helpers
{
    public class CreateRateLimiter
    {
        private static readonly ResourceLimitter resourceLimitter;
        private static readonly MixedResourceLimitter mixedResourceLimitter;
        private static readonly IAllowRequest allowRequestPerTimespan;
        private static readonly IAllowRequest allowTimespanSinceLastCall;
        private static readonly IAllowRequest allowUSRequestLimit;
        private static readonly IAllowRequest allowEUReqiestLimit;


        public static ResourceLimitter ResourceLimitter
        {
            get { return resourceLimitter; }
        }

        public static MixedResourceLimitter MixedResourceLimitter
        {
            get { return mixedResourceLimitter; }
        }

        public static IAllowRequest AllowRequestPerTimespan
        {
            get { return allowRequestPerTimespan; }
        }

        public static IAllowRequest AllowTimespanSinceLastCall
        {
            get { return allowTimespanSinceLastCall; }
        }

        public static IAllowRequest AllowUSRequestLimit
        {
            get { return allowUSRequestLimit; }
        }

        public static IAllowRequest AllowEUReqiestLimit
        {
            get { return allowEUReqiestLimit; }
        }

        static CreateRateLimiter()
        {
            resourceLimitter = new ResourceLimitter();            

            allowRequestPerTimespan = new RequestPerTimespan(10, TimeSpan.FromSeconds(60));
            resourceLimitter.Configure(allowRequestPerTimespan);

            allowTimespanSinceLastCall = new TimespanSinceLastCall(TimeSpan.FromSeconds(40));
            resourceLimitter.Configure(allowTimespanSinceLastCall);

            IEnumerable<IAllowRequest> rules = new List<IAllowRequest>();
            rules.Append(allowRequestPerTimespan);
            rules.Append(allowTimespanSinceLastCall);

            mixedResourceLimitter = new MixedResourceLimitter(rules);


            allowUSRequestLimit = new USRequestLimitRule(50, TimeSpan.FromSeconds(30));
            resourceLimitter.Configure(allowUSRequestLimit);

            allowEUReqiestLimit = new EURequestLimitRule(TimeSpan.FromSeconds(60));
            resourceLimitter.Configure(allowEUReqiestLimit);
        }
    }
}
