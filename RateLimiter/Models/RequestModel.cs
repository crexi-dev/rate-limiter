using RateLimiter.Enums;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Models
{
    public class RequestModel
    {
        //This must be provided in order to rate limit. This could be an endpoint, or whatever the API wants it to be. 
        public string Source { get; set; } 
        /* This UserID would be a unique identifier of the Client making the request
         * Every request must have a UserID, otherwise you would be ratelimiting the entire resource regardless of client. 
         * I'm assuming that the API already has done the work and has performed token introspection and figured out what user is calling this API. */
        public string UserID { get; set; }
        public DateTime TimeRequested { get; set; }
        public string? Location { get; set; }
        public RequestModel(string source, string userID, DateTime timeRequested, string location = "")
        {
            Source = source;
            UserID = userID;
            TimeRequested = timeRequested;
            Location = location;
        }
        public RequestModel()
        {
            Source = "";
            Location = "";
            UserID = "";
        }
    }
}
