using RateLimiter.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Services
{
    public interface IAccessService
    {
        long Add(Access access);
        IList<Access> GetAll();
        Access GetById(int id);
        void Delete(Access access);
        void Delete(int id);
        bool IsAccessAllowedPerTimeSpan(int accountId, string token, int timeSpanInSeconds, bool isLimitPerAccount = false);
        bool IsRateLimitReached(int accountId, string token, int timeSpanInSeconds, int maxRequestsPerTimeSpan, bool isLimitPerAccount = false);
        bool IsAccessBlockedByIp(string ipRangeStart, string ipRangeEnd, IPAddress clientIp);
    }
}
