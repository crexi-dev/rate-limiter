using RateLimiter.Rules;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RequestProcessor : IRequestProcessor
    {
        /// <summary>
        /// Sample implementation of the request process
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The response with processing result</returns>
        public async Task<Response> DoRequestAsync(Request request)
        {
            await Task.Delay(request.ProcessDurationMs);
            return new Response() { IsSuccessful = true };
        }
    }
}
