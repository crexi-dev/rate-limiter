using System.Collections.Generic;
using RateLimiter.DataModel;

namespace RateLimiter.DataAccess.Query
{
    public interface IQueryRepo
    {
        List<Client> GetClients();
        List<Resource> GetResources();
        List<Rule> GetRules();
        List<ClientResourceRuleMapping> GetClientResourceRuleMappings();
    }
}
