using RateLimiter.Interface;
using System.Threading.Tasks;

namespace RateLimiter.Model.Rules
{
    public class USBasedRequestRule : IRuleBase
    {
        public string Resource { get; set; }

        public bool TargetResult { get; set; } = true;
        public async Task<bool> Evaluate(IClient client, IResourceRequest currentRequest)
        {
            bool resourceMatched = (Resource == null) || (Resource == currentRequest.Resource);
            if (!resourceMatched)
                return true;

            var isUSBased = client.Region == "US";
            return (isUSBased == TargetResult);
        }
    }
}
