using RateLimiter.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Store
{
    public class Store
    {
        public static List<RateLimiterRule> Rules = new List<RateLimiterRule>();

        public static List<Resource> Resources = new List<Resource>();

        public static List<Client> Clients = new List<Client>();
    }
}
