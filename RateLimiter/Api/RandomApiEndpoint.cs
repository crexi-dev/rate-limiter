using System;
using System.Threading.Tasks;

namespace RateLimiter.Api
{
    public sealed class RandomApiEndpoint : IApiEndpoint<int>
    {
        public async Task<Response<int>> ActionAsync(string accessToken)
        {
            await Task.Delay(2000);

            var random = new Random().Next(10);

            return new Response<int> { Data = random };
        }
    }
}
