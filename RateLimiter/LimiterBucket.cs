using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class LimiterBucket : ILimiterBucket
    {
        public int Capacity{ get;}
        public int Counter { get; set; }
        public DateTime CreatedAt { get; }
        public DateTime UpdatedAt { get; set; }
        public int RefreshRate { get; }

        public LimiterBucket(int capacity, int refreshRate) 
        {
            Capacity = capacity;
            Counter = capacity;
            RefreshRate = refreshRate;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            UpdateTokens();
        }

        public virtual void ProcessRequest()
        { 
            if(Counter >0 ) Counter--;
            UpdatedAt = DateTime.Now;
        }
        public virtual void UpdateTokens()
        {
            Counter = Capacity;
        }
    }
}
