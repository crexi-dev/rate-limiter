using Microsoft.AspNetCore.Http;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Providers
{
    public class RulesProvider
    {
        private readonly HttpContext _httpContext;
        private readonly RateLimiterConfigModel _rateLimiterConfigModel;

        public RulesProvider(HttpContext httpContext, RateLimiterConfigModel rateLimiterConfigModel )
        {
            _httpContext = httpContext;
            _rateLimiterConfigModel = rateLimiterConfigModel;
        }

        public bool evaluateRules()
        {
            List<bool> ruleResults = new List<bool>();



            var endpointRules = GetEndpointRules();//read rules from config for this endpoint 
            if (endpointRules != null && endpointRules.Count>0)
            {
                foreach (var rule in endpointRules)
                {
                    bool res = getRuleResult(rule, _httpContext);//execute those rules for this endpoint
                    ruleResults.Add(res);
                }
            }
            //run default rule if there are no rules for this endpoint
            else if (_rateLimiterConfigModel.DefaultRule != null && !string.IsNullOrEmpty(_rateLimiterConfigModel.DefaultRule.Name))
            {
                bool res = getRuleResult(_rateLimiterConfigModel.DefaultRule, _httpContext);
                ruleResults.Add(res);
            }
            else
            {
                ruleResults.Add(true);//return true if there are no rules at all
            }

            return !ruleResults.Contains(false);//return false if any of the rules was violated

        }
        private List<RuleSettingModel> GetEndpointRules() //find rules suitable for this endpoint
        {
            List<RuleSettingModel> endpointRules = new List<RuleSettingModel>();
            string endpoint = _httpContext.Request.Path;
          
            int index = endpoint.LastIndexOf("/");
            while (endpoint.Contains("/"))
            {
                endpointRules.AddRange(_rateLimiterConfigModel.Rules.Where(x => endpoint==x.Endpoint));
                if (endpointRules.Count > 0)
                    break;
                index = endpoint.LastIndexOf("/");
                endpoint = endpoint.Substring(0, index);
            }
            return endpointRules;
        }

        private bool getRuleResult(RuleSettingModel ruleData, HttpContext httpContext)//dynamically Create and run rules written in config file
        {
            Type t = Type.GetType("RateLimiter.Rules." + ruleData.Name);


            if (t == null)
                throw new Exception($"Could not Find Rule Named: {ruleData.Name}");

            bool methodResult;
            var rule = Activator.CreateInstance(t);

            if (rule == null)
                throw new Exception($"Could not Instanciate Rule Named: {ruleData.Name}");

            MethodInfo method = rule.GetType().GetMethod("Evaluate");
            if (method == null)
                throw new Exception($"Rule Named: {ruleData.Name} is not implemented correctly");

            object[] parametersArray = new object[] { ruleData, httpContext };
            try
            {
                methodResult = (bool)method.Invoke(rule, parametersArray);

            }
            catch(TargetInvocationException ex)
            {
                throw new Exception(ex.InnerException.Message);//remove wrapper created by reflection
            }
            return methodResult;

        }

    }
}
