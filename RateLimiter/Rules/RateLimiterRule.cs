using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public enum EnumFilterMatchMode { Any = 0, All = 1};
    public class RateLimiterRule
    {
        public IRateLimiterStrategy Strategy { get; private set; }
        public List<IRateLimiterFilter> Filters { get; private set; }

        public RateLimiterRule(IRateLimiterStrategy strategy, List<IRateLimiterFilter> filters = null)
        {
            Strategy = strategy;
            Filters = filters;
        }

        public RateLimiterRule(IRateLimiterStrategy strategy, IRateLimiterFilter filter)
        {
            Strategy = strategy;
            Filters = new List<IRateLimiterFilter> { filter };
        }


        public bool FiltersMatched(List<IRateLimiterFilter> targetFilters, EnumFilterMatchMode matchMode = EnumFilterMatchMode.Any)
        {
            // check case when targetFilters are null or Rule Filters are null
            if (targetFilters == null)
                return Filters == null;
            else if (Filters == null)
                return true;
            int countMatchedFilters = 0;

            foreach (var targetFilter in targetFilters)
            {
                bool filterMatched = isMatched(targetFilter);
                if (filterMatched)
                {
                    if (matchMode == EnumFilterMatchMode.Any)
                        return true;
                    else if (matchMode == EnumFilterMatchMode.All)
                        countMatchedFilters++;
                }
            }

            if (matchMode == EnumFilterMatchMode.All && countMatchedFilters == targetFilters.Count)
                return true;
            
            return false;
        }

        private bool isMatched(IRateLimiterFilter targetFilter)
        {
            foreach(var filter in Filters)
            {
                if (filter.Match(targetFilter))
                    return true;
            }

            return false;
        }
    }
}
