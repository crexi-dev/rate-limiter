using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RateLimiter
    {
        #region - Variables -
        private int CounterActions { get; set; }
        private int MaxActions { get; set; }

        private DateTime LastAction { get; set; }

        private TimeSpan MinTimeBetweendActions { get; set; }
        #endregion

        #region - Contructors -
        public RateLimiter(int maxActionInPeriod, TimeSpan? minTimeBetweenActions = null)
        {
            CounterActions = 0;
            LastAction = DateTime.MinValue;
            MaxActions = maxActionInPeriod;

            MinTimeBetweendActions = minTimeBetweenActions == null ? new TimeSpan(0) : minTimeBetweenActions.Value;
        }
        #endregion

        #region - Private Methods -
        public void PerformAction()
        {
            CounterActions++;
            LastAction = DateTime.Now;
        }

        public bool CanPerformAction()
        {
            if (CounterActions >= MaxActions)
                return false;

            if (LastAction.Add(MinTimeBetweendActions) > DateTime.Now)
                return false;

            return true;
        }

        //private static RateLimiter GetRateLimiter(string actionName, string requestIdentifier, TimeSpan period, int maxAction, TimeSpan? minTime = null)
        //{
            
        //}


        #endregion
    }
}
