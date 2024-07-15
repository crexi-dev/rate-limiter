using RateLimiter.Interface;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimiter.Model.Rules
{
    public class MaxPerTimeSpanRule : IRuleBase
    {


        public string? Resource { get; set; }
        public int maxcount { get; set; }
        public int timespan { get; set; }

        public async Task<bool> Evaluate(IClient client, IResourceRequest currentRequest)
        {
            var thisRequestTime = currentRequest.DateTime;
            var filterTime = thisRequestTime.AddSeconds(-timespan);
            int previousRquestsCount;
            if (Resource == null) // rule is max number of request whichever resource
                previousRquestsCount = client.resourceRequests.Where(r => r.DateTime >= filterTime).Count();
            else if (Resource == currentRequest.Resource)
                previousRquestsCount = client.resourceRequests.Where(r => r.DateTime >= filterTime && r.Resource == Resource).Count();
            else // rule does not apply to this request, so pass
                return true;
            return previousRquestsCount < maxcount;
        }


    }
}
