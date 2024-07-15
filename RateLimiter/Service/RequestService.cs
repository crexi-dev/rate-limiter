using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateLimiter.Interface;
using RateLimiter.Interface;
using RateLimiter.Model;

namespace RateLimiter.Service
{
    public class RequestService : IRequestService
    {
        private List<ReqClient> reqClients = new List<ReqClient>();
        public IClient GetClient(string token)
        {
            // In a production environment this method would find and lookup the client record from the token by whatever
            // lookup means was designed 
            // the reqClients list might be a Redis table used as a cache and fast data retrieval mechanism

            if (reqClients.Exists(r => r.Token == token))
                return reqClients.FirstOrDefault(r => r.Token == token);

            var client = new ReqClient(token);
            // obviously convention for demo purposes
            if (token.ToUpper().StartsWith("US"))
                client.Region = "US";
            else
                client.Region = "EU";
            client.Subscription = token[3].ToString();
            reqClients.Add(client);
            return client;
        }
    }
}
