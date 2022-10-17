using System.Collections.Generic;
using RateLimiter.DataModel;

namespace RateLimiter.DataAccess.Query
{
    public class QueryRepo : IQueryRepo
    {
        public QueryRepo()
        {

        }

        public List<Client> GetClients()
        {
            List<Client> lstClient = new List<Client>();

            lstClient.Add(new Client() { ClientId = 1, ClientName = "Client1" });
            lstClient.Add(new Client() { ClientId = 2, ClientName = "Client2" });

            return lstClient;
        }

        public List<Resource> GetResources()
        {
            List<Resource> lstResource = new List<Resource>();

            lstResource.Add(new Resource() { ResourceId = 1, ResourceName = "Resource1", Url = @"https://Resource1.com" });
            lstResource.Add(new Resource() { ResourceId = 2, ResourceName = "Resource2", Url = @"https://Resource2.com" });

            return lstResource;
        }

        public List<Rule> GetRules()
        {
            List<Rule> lstRule = new List<Rule>();

            lstRule.Add(new Rule() { RuleId = 1, RuleDescription = "Requests per minute" });
            lstRule.Add(new Rule() { RuleId = 2, RuleDescription = "Timespan since last call (in seconds)" });

            return lstRule;
        }

        public List<ClientResourceRuleMapping> GetClientResourceRuleMappings()
        {
            List<ClientResourceRuleMapping> lstMappings = new List<ClientResourceRuleMapping>();

            lstMappings.Add(new ClientResourceRuleMapping() { ClientId = 1, ResourceId = 1, RuleId = 1, Value = 5 });
            lstMappings.Add(new ClientResourceRuleMapping() { ClientId = 2, ResourceId = 2, RuleId = 2, Value = 10 });

            return lstMappings;
        }
    }
}
