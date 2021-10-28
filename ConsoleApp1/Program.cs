using RateLimiter;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var tt = new Api();

            var result = tt.ResourceA("giotoken");
            var result2 = tt.ResourceA("giotoken");
            var result3 = tt.ResourceA("giotoken");
        }
    }
}

