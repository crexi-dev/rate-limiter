using RateLimits.History;
using RateLimits.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimits.RateLimits
{
    public class RateLimiter : IRateLimiter
    {
        private IHistoryRepository _historyRepository;
        public RateLimiter(IHistoryRepository historyRepository)
        {
            _historyRepository = historyRepository;
        }
        public bool HasAccess(string accessToken, string resourceName, string region, params IRule[] rules)
        {
            if (rules.Length == 0)
            {
                throw new ArgumentNullException();
            }
            var history =_historyRepository.Get(accessToken, resourceName);
            var hasAccess = rules.All(i => i.Execute(history,region));
            if (hasAccess) _historyRepository.Add(accessToken, resourceName, region);
            return hasAccess;
        }
    }
}
