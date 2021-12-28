using System;
using System.Collections.Generic;

namespace RateLimiter.Entities
{
    public class ClientModel
    {
        public string Token { get; set; }
        public CountryModel Country { get; set; }
        public int LastRequestId { get; set; }
        public DateTime? LastCallDate { get; set; }
        public List<object> Rules { get; set; }

        public ClientModel()
        {
            Country = new CountryModel();
            Rules = new List<object>();
        }
    }
}
