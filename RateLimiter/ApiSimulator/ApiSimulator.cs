using System;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class ApiSimulator : IApiSimulator
    {
        private readonly Settings? _settings;
        private readonly IRateLimitLibrary _rateLimit;
        public ApiSimulator(IOptions<Settings> settings, IRateLimitLibrary rateLimit)
        {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _rateLimit = rateLimit;
        }

        public void SimulateApiCall(string apiEndpointName)
        {
            var userToken = "authTokenA";
            bool isValid = _rateLimit.IsRateLimitAccepted(userToken, apiEndpointName);
            if (isValid)
            {
                ExecuteApiCall(apiEndpointName);
            }
            else
            {
                Console.WriteLine($"REQUEST REJECTED! {apiEndpointName} exceeded call limit for this user");
            }
            return;
        }

        public void ExecuteApiCall(string apiEndpointName)
        {
            switch(apiEndpointName)
            {
                case "ApiResource1" :
                    ApiResource1();
                    break;
                case "ApiResource2" :
                    ApiResource2();
                    break;
                case "ApiResource3" :
                    ApiResource3();
                    break;
                default :
                    Console.WriteLine($"Api Resource does not exist: {apiEndpointName}");
                    break;
            }
        }

        public void ApiResource1()
        {
            Console.WriteLine($"ApiResource1 Successfully Executed!");
        }

        public void ApiResource2()
        {
            Console.WriteLine($"ApiResource2 Successfully Executed!");
        }

        public void ApiResource3()
        {
            Console.WriteLine($"ApiResource3 Successfully Executed!");
        }
    }
}