using RateLimiter.Entities;
using RateLimiter.Entities.Rules;
using System.Linq;

namespace RateLimiter.BLL
{
    public static class RulesManager
    {
        public static bool CheckRules(RequestModel request)
        {
            var client = ClientHelper.GetClientByToken(request.ClientToken);
            if (client == null)
            {
                return false;
            }

            if (!client.Rules.Any())
            {
                return true;
            }

            foreach (var rule in client.Rules)
            {
                if (rule != null)
                {
                    if (rule.GetType() == typeof(CountryRuleModel))
                    {
                        if(!((CountryRuleModel)rule).CheckRule(client))
                        {
                            return false;
                        }
                    }
                    else if (rule.GetType() == typeof(LastCallRuleModel))
                    {
                        if(!((LastCallRuleModel)rule).CheckRule(client))
                        {
                            return false;
                        }
                    }
                    else if (rule.GetType() == typeof(LastCallForCountryRuleModel))
                    {
                        if (!((LastCallForCountryRuleModel)rule).CheckRule(client))
                        {
                            return false;
                        }

                    }
                    else if (rule.GetType() == typeof(RequestCountForPeriodRuleModel))
                    {
                        if (!((RequestCountForPeriodRuleModel)rule).CheckRule(client))
                        {
                            return false;
                        }
                    }
                    else if (rule.GetType() == typeof(RequestCountRuleModel))
                    {
                        if (!((RequestCountRuleModel)rule).CheckRule(client))
                        {
                            return false;
                        }

                    }
                    else if (rule.GetType() == typeof(RequestCountForCountryRuleModel))
                    {
                        if (!((RequestCountForCountryRuleModel)rule).CheckRule(client))
                        {
                            return false;
                        }
                    }
                    else if (rule.GetType() == typeof(RequestCountForCountryForPeriodRuleModel))
                    {
                        if (!((RequestCountForCountryForPeriodRuleModel)rule).CheckRule(client))
                        {
                            return false;
                        }
                    }
                    else if (rule.GetType() == typeof(PassedTimeRuleModel))
                    {
                        if (!((PassedTimeRuleModel)rule).CheckRule(client))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
