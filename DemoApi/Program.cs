using Microsoft.Extensions.Configuration;
using RateLimiter;
using System;
using System.Threading;

namespace DemoApi
{
    public class Program
    {
        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Config.Model = config.Get<ConfigModel>();
            var id = Guid.NewGuid();
            for (int i = 0; i < 100; i++) 
            {
                TestRequest(id.ToString(), i);
                Thread.Sleep(10);
            }
        }

        public static void TestRequest(string key, int i) 
        {
            if (RateLimiterService.AllowRequest(key, Config.GetClientConfig(new ThrottleSettings() { ThrottleMaxRequestPerTime = new() { allow = true, region = "EU" } })))
            {
                Console.WriteLine("Allow request");
            }
            else 
            {
                Console.WriteLine("Blocked request");
            }
        }
    }
}
