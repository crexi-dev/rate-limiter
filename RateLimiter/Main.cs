using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RateLimiter
{
    public class Main
    {
        public List<CallDetails> callDetails = new List<CallDetails>();
        public void RateLimiter()
        {
            var user = new User();
            callDetails.Add(new CallDetails
            {
                UserId = user.Id,
                ApiCallTime = DateTime.Now
            });

            if (RuleA(user.Id))
            {
                user.GetResourceA();
            }

            if (RuleB(user.Id))
            {
                user.GetResourceB();
            }

            if (RuleA(user.Id) && RuleB(user.Id))
            {
                user.GetResourceC();
            }
        }

        // 1 houer must be past after last call
        public bool RuleA(int id)
        {
            TimeSpan timeSpan = new TimeSpan(1, 0, 0);
            var currentUserLastCall = callDetails.Where(x => x.UserId == id).Select(x => x.ApiCallTime).Max();
            if (DateTime.Now - currentUserLastCall < timeSpan)
            {
                return false;
            }
            return true;
        }

        //max 10 request in last 24 houers
        public bool RuleB(int id)
        {
            int requestQunatity = 10;
            TimeSpan timeSpan = new TimeSpan(24, 0, 0);
            var requstsInTimeSpan = (from r in callDetails
                                     where r.UserId == id && (DateTime.Now - r.ApiCallTime) < timeSpan
                                     select r.UserId).Count();


            if (requstsInTimeSpan > requestQunatity)
            {
                return false;
            }
            return true;
        }
    }
}
