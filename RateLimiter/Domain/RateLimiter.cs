
using RateLimiter.Models;
using RateLimiter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace RateLimiter.Domain
{
    public static class RateLimiter
    {
        private static Queue<RequestModel> queueEU = new Queue<RequestModel>();
        private static Queue<RequestModel> queueUS = new Queue<RequestModel>();
        

        private static List<RuleModel> rules;

        static RateLimiter()
        {
            rules = RulesService.GetRules();
        }

        public static void EnqueuRequest(RequestModel request)
        {
            if (request.ClientLocation == Models.Enums.ELocation.EU)
                queueEU.Enqueue(request);
            if (request.ClientLocation == Models.Enums.ELocation.EU)
                queueUS.Enqueue(request);
        }

        public static bool Check(RequestModel request)
        {
            //if there is any requests in queue and there is no way to process the reqeust than wait
            bool isRulesPassed = CheckRule(rules, request);

            return isRulesPassed;
        }

        public static bool CheckRule(List<RuleModel> rules, RequestModel request)
        {
            if (request.ClientLocation == Models.Enums.ELocation.EU)
            {
                if (queueEU.Count > rules.FirstOrDefault(/*condition*/).RequestFrequency)
                    return false;
                else
                    return true;
            }

            if (request.ClientLocation == Models.Enums.ELocation.US)
            {
                if (queueEU.Count > rules.FirstOrDefault(/*condition*/).RequestFrequency)
                    return false;
                else
                    return true;
            }

            return false;
        }
    }
}
