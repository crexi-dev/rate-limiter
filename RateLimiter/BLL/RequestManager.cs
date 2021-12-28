using RateLimiter.DAL;
using RateLimiter.Entities;
using System;
using System.Linq;

namespace RateLimiter.BLL
{
    //Main class
    public static class RequestManager
    {
        //Method is calling to check the requests
        public static bool SendRequest(RequestModel request)
        {
            if(request != null && !string.IsNullOrWhiteSpace(request.ClientToken))
            {
                var result = RulesManager.CheckRules(request);
                //database update
                var newRequest = new ClientRequestModel { Token = request.ClientToken, RequestDate = DateTime.Now, RequestId = DatabaseSimulator.ClientRequests.Select(x => x.RequestId).DefaultIfEmpty().Max() + 1 };
                DatabaseSimulator.ClientRequests.Add(newRequest);
                var client = DatabaseSimulator.Clients.FirstOrDefault(x => x.Token == request.ClientToken);
                DatabaseSimulator.Clients.Remove(client);
                client.LastCallDate = DateTime.Now;
                client.LastRequestId = newRequest.RequestId;
                DatabaseSimulator.Clients.Add(client);

                return result;
            }

            return false;
        }
    }
}
