using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RateLimiterManager
    {
        #region - Variables -
        private LimiterCache _cache = new LimiterCache();
        #endregion

        #region - Public Methods - 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionName">Name of the action or function</param>
        /// <param name="requestIdentifier">Unique Identifier for request, like IP address or user</param>
        /// <param name="period">A specific time period in which no more that maxActions can be performed</param>
        /// <param name="maxActions">Max number of actions in given period</param>
        /// <param name="minTimeBetweenActions">Optional, minimum time between actions</param>
        /// <returns></returns>
        public bool Check(string actionName, string requestIdentifier, TimeSpan period, int maxActions, TimeSpan ? minTimeBetweenActions = null)
        {
            var limiter = _cache.GetOrCreate(GetRateLimiterKey(actionName, requestIdentifier), period, maxActions, minTimeBetweenActions);

            if (limiter.CanPerformAction())
            {
                limiter.PerformAction();
                return true;
            }

            return false;
        }
        #endregion
        
        #region - Private Methods -
        private string GetRateLimiterKey(string actionName, string requestIdentifier)
        {
            return $"RateLimiter.{actionName}.{requestIdentifier}";
        }
        #endregion  
    }
}
