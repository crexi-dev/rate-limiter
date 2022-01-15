using System.Threading.Tasks;

namespace RateLimiter.Api
{
    public interface IApiEndpoint<T>
    {
        Task<Response<T>> ActionAsync(string accessToken);
    }
}
