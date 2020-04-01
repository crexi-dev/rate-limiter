using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Classes
{
    public class RateLimiterManager : IRateLimiterManager
    {
        // map API_Function_Name to a list of Rules 
        public Dictionary<string, List<IRule>> ApiAndRulesMap = new Dictionary<string, List<IRule>>();

        public List<IClientRequest> RequestsLog { get; } = new List<IClientRequest>();

        public void AddRule(string apiName, IRule rule)
        {
            try
            {
                if (ApiAndRulesMap.ContainsKey(apiName))
                    ApiAndRulesMap[apiName].Add(rule);
                else
                    ApiAndRulesMap.Add(apiName, new List<IRule>() { rule });
            }
            catch (Exception ex)
            {
                // Log/Manage exception 
            }
        }

        public bool Validate(string ApiName, IClientRequest request)
        {
            try
            {
                RequestsLog.Add(request);

                var rulesToApply = ApiAndRulesMap[ApiName];
                foreach (var rule in rulesToApply)
                {
                    if (!rule.Validate(request, this))
                        return false;
                }

                return true;
            }
            catch (Exception ex) 
            {
                return false;
                // Log/Manage exception 
            }
        }
    }
}
