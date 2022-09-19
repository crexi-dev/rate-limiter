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
        public async Task<Response> DoRequestAsync(object request)
        {
            await Task.Delay((int)request);
            return new Response() { IsSuccessful = true };
        }
    }
}
