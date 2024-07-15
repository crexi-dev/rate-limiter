using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateLimiter.Interface;
using RateLimiter.Model;

namespace RateLimiter.Model.Rules
{
    public class RuleList : IRuleBase
    {
        public string Resource { get; set; }
        public List<IRuleBase> Rulelist { get; } = new List<IRuleBase>();
        public List<bool> RuleResults { get; } = new List<bool>();
        public string ListOperand { get; set; } = "And";

        public async Task<bool> Evaluate(IClient client, IResourceRequest req)
        {
            RuleResults.Clear();
            foreach (IRuleBase rule in Rulelist)
            {
                RuleResults.Add(await rule.Evaluate(client, req));

            }

            return await GetEvalResults();
        }

        private async Task<bool> GetEvalResults()
        {
            bool result;
            switch (ListOperand.ToLower())
            {
                case "or":
                case "||":
                    result = false;
                    foreach (var item in RuleResults)
                    {
                        result = result || item;
                    }
                    break;
                case "and":
                case "&&":
                default:
                    result = true;
                    foreach (var item in RuleResults)
                    {
                        result = result && item;
                    }
                    break;

            }
            return result;
        }
    }
}
