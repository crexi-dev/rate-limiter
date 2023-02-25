using System;

namespace RateLimiter
{
    class Program
    {
        static void Main(string[] args)
        {
            var requestRepository = new InMemoryRequestRepository();
            var limiter = new RateLimiter(requestRepository);
            var perTimespanStrategy = new PerTimespanRateLimitStrategy(10, TimeSpan.FromSeconds(10));
            limiter.AddLimitationRule("x", perTimespanStrategy);

            for (int i = 0; i < 20; i++)
            {
                if (limiter.TryMakeRequest("john","x"))
                {
                    Console.WriteLine("Request succeeded");
                }
                else
                {
                    Console.WriteLine("Request rate limit exceeded");
                }
            }
        }
    }
}
