using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    public class FakeEndpointService : IEndpointService
    {
        private readonly Dictionary<string, string> _endpointsDictionary;

        public FakeEndpointService()
        {
            _endpointsDictionary = new Dictionary<string, string>(){
                { "TokenTest1", "USBasedEndpoint1" },
                { "TokenTest2", "USBasedEndpoint2" },
                { "TokenTest3", "EUBasedEndpoint1" }
            };
        }

        public Task<string> GetEndPoint(string accessToken)
        {
            return Task.FromResult(_endpointsDictionary[accessToken]);
        }
    }
}
