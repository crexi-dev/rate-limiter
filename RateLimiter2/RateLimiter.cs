using RateLimiter.Resources;
using RateLimiter.Stores;

namespace RateLimiter
{
    public class RateLimiter
    {
        private readonly ResourceStore ResourceStore;
        private readonly ClientRequestStore RequestStore;

        public RateLimiter()
        {
            ResourceStore = new ResourceStore();
            RequestStore = new ClientRequestStore(); 
        }

        public bool EvaluateClientRequest(string clientToken, string path, string httpVerb)
        {
            var r = new Resource(path, httpVerb);

            // search resrouce store for the current request
            var rateLimitedResources = ResourceStore.Resources.Where(r => r.Key == r.Key);

            // if resource requested is not rate limited then allow it
            if (!rateLimitedResources.Any()) return true;

            // resources 
            foreach (var resource in rateLimitedResources)
            {
                // if no rules defined for resource then allow request
                if (!resource.Rules.Any()) return true;

                // if no past client requests exist for the resource then add this one and allow request
                if (!RequestStore.Any(clientToken, resource.Key))
                {
                    RequestStore.AddRequest(clientToken, resource.Key);
                    return true;
                }

                // list of past requests this client has made
                var requests = RequestStore.GetRequests(clientToken, resource.Key);

                // list of rules to evaluate
                var rules = resource.Rules.ToList();
                rules.Sort();

                // setup for rule processing
                var terminateRuleProcessing = false;
                var result = true;

                // process all rules defined for the resource
                foreach (var rule in rules)
                {
                    // evalute the next rule
                    result = rule.Evaluate(resource, clientToken, requests, ref terminateRuleProcessing);

                    // terminate if rule requested it
                    if (terminateRuleProcessing) break;
                }

                // add this request to the store
                RequestStore.AddRequest(clientToken, resource.Key);

                return result;
            }
            return true;
        }
    }
}
