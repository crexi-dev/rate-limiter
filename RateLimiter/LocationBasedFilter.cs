using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class LocationBasedFilter : IRateLimiterFilter
    {
        public string CountryCode { get; set; }

        public string PostalCode { get; set; }

        public string City { get; set; }

        public bool Match(IRateLimiterFilter targetFilter)
        {
            if (!(targetFilter is LocationBasedFilter))
                throw new ArgumentException("targetFilter should be type of LocationBasedFilter");
           
            var target = (LocationBasedFilter)targetFilter;

            return CountryCode == target.CountryCode
                && PostalCode == target.PostalCode
                && City == target.City;
        }

    }
}
