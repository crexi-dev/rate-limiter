using RateLimiter.Data;
using RateLimiter.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Services
{
    public class AccessService : IAccessService
    {
        public IInMemoryStorageManager _storageManager { get; private set; }

        public AccessService(IInMemoryStorageManager storageManager) {
            _storageManager = storageManager;
        }

        public long Add(Access access) {
            access.ID = GenerateId();
            _storageManager.Add(access);
            return access.ID;
        }

        public IList<Access> GetAll()
        {
            return _storageManager.GetAll();
        }
        public Access GetById(int id)
        {
            return _storageManager.GetById(id);
        }

        public void Delete(Access access)
        {
            _storageManager.Delete(access);
        }

        public void Delete(int id) {
            _storageManager.Delete(id);
        }

        public bool IsAccessAllowedPerTimeSpan(int accountId, string token, int timeSpanInSeconds, bool isLimitPerAccount = false)
        {
            Func<Access, bool> func;
            if (isLimitPerAccount)
            {
                func = new Func<Access, bool>(a => a.Account == accountId && a.Date > DateTime.Now.Subtract(new TimeSpan(0, 0, 0, timeSpanInSeconds)));
            }
            else
            {
                func = new Func<Access, bool>(a => a.Token == token && a.Date > DateTime.Now.Subtract(new TimeSpan(0, 0, 0, timeSpanInSeconds)));
            }

            return _storageManager.Count(func) == 0;
        }

        public bool IsRateLimitReached(int accountId, string token, int timeSpanInSeconds, int maxRequestsPerTimeSpan, bool isLimitPerAccount = false)
        {
            Func<Access, bool> func;
            if (isLimitPerAccount)
            {
                func = new Func<Access, bool>(a => a.Account == accountId && a.Date > DateTime.Now.Subtract(new TimeSpan(0, 0, 0, timeSpanInSeconds)));
            }
            else {
                func = new Func<Access, bool>(a => a.Token == token && a.Date > DateTime.Now.Subtract(new TimeSpan(0, 0, 0, timeSpanInSeconds)));
            }
            
            return  _storageManager.Count(func) >= maxRequestsPerTimeSpan;
        }
        public bool IsAccessBlockedByIp(string ipRangeStart, string ipRangeEnd, IPAddress clientIp)
        {
            IPAddress startIp;
            IPAddress endIp;

            if (!IPAddress.TryParse(ipRangeStart, out startIp) || !IPAddress.TryParse(ipRangeEnd, out endIp))
            {
                //couldn't parse IP range. Either return a bool by default, throw an Exception or log the info for the config to be fixed.
                return false;
            }

            if (clientIp.AddressFamily != startIp.AddressFamily)
            {
                return false;
            }

            byte[] clientIpBytes = clientIp.GetAddressBytes();
            byte[] startIpBytes = startIp.GetAddressBytes();
            byte[] endIpBytes = endIp.GetAddressBytes();

            //bool lowerBoundary = true, upperBoundary = true;

            //for (int i = 0; i < startIpBytes.Length && (lowerBoundary || upperBoundary); i++)
            //{
            //    if ((lowerBoundary && clientIpBytes[i] < startIpBytes[i]) || (upperBoundary && clientIpBytes[i] > endIpBytes[i]))
            //    {
            //        return false;
            //    }

            //    lowerBoundary &= (clientIpBytes[i] == startIpBytes[i]);
            //    upperBoundary &= (clientIpBytes[i] == endIpBytes[i]);
            //}

            for (int i = 0; i < startIpBytes.Length; i++)
            {
                if ((clientIpBytes[i] < startIpBytes[i]) || (clientIpBytes[i] > endIpBytes[i]))
                {
                    return false;
                }
            }

            return true;
        }

        #region [Private Methods]

        private long GenerateId() {
            return _storageManager.GetLastId() + 1;
        }

        #endregion [Private Methods]
    }
}
