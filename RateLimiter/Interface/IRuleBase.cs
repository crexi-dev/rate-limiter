

using System.Threading.Tasks;

namespace RateLimiter.Interface
{
    public interface IRuleBase
    {
        string Resource { get; set; }
        Task<bool> Evaluate(IClient client, IResourceRequest currentRequest);
    }
}
