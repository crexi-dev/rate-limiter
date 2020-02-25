using RateLimiter.Limiter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            //LimitResolver.Instance.AddLimit("US", 10, 3); // A type - 3 requests in 10 minutes
            //LimitResolver.Instance.AddLimit("EU", 5, 1); // B type - 1 request in 5 minutes

            //LimitResolver.Instance.AddTokenLimit("A", "US");

            //LimitResolver.Instance.AddTokenLimit("B", "EU");

            //LimitResolver.Instance.AddTokenLimit("AB", "US");
            //LimitResolver.Instance.AddTokenLimit("AB", "EU");


            LimitResolver.Instance.AddLimit("US", 5, 3); // A type - 3 requests in 5 seconds

            LimitResolver.Instance.AddTokenLimit("A", "US");

            Enumerable.Range(1, 4).AsParallel().ForAll(t =>
            {
                Thread.Sleep(t * 100);
                Console.WriteLine($"{t} {LimitResolver.Instance.NewQuery("A")}");
            });
            Thread.Sleep(5000);
            Console.WriteLine(LimitResolver.Instance.NewQuery("A"));
        }
    }
}
