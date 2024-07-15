using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using RateLimiter.Interface;
using RateLimiter.Model;

namespace RateLimiter.Model.Rules
{
    public class MinTimeSpanLapsRule : IRuleBase
    {
        public string? Resource { get; set; }
        public int Laps { get; set; } //assume seconds but could be configurable


        public async Task<bool> Evaluate(IClient client, IResourceRequest currentRequest)
        {
            ResourceRequest? lastResourceRequest = null;
            if (Resource == null) // rule is max number of request whichever resource
                lastResourceRequest = (ResourceRequest)client.resourceRequests?.LastOrDefault();

            else if (Resource == currentRequest.Resource)
                lastResourceRequest = (ResourceRequest)client.resourceRequests?.LastOrDefault(r => r.Resource == Resource);

            if (lastResourceRequest != null)
            {
                var diffOfDates = currentRequest.DateTime - lastResourceRequest.DateTime;
                return diffOfDates.TotalSeconds > Laps;
            }
            return true;

        }
    }
}
