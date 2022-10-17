using System.Collections.Generic;
using System.Linq;
using RateLimiter.DataModel;

namespace RateLimiter.Extensions
{
    public static class RequestDataExtensions
    {
        public static List<ClientResourceRuleMapping> GetRelevantMappings(this RequestData requestData)
        {
            return requestData?.Mappings?.Where(x => x.ClientId == requestData.ClientRequest?.ClientId && x.ResourceId == requestData.ClientRequest?.ResourceId).ToList();
        }

        public static ClientRequestHistory GetRelevantLatestHistoryData(this RequestData requestData)
        {
            return requestData?.HistoryData?.Where(x => x.ClientId == requestData.ClientRequest?.ClientId && x.ResourceId == requestData.ClientRequest?.ResourceId).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
        }

        public static int GetRuleValue(this RequestData requestData, int ruleId)
        {
            return (requestData?.Mappings?.Where(x => x.ClientId == requestData.ClientRequest?.ClientId && x.ResourceId == requestData.ClientRequest?.ResourceId && x.RuleId == ruleId).FirstOrDefault().Value) ?? 0;
        }

        public static void AddHistoryData(this RequestData requestData, ClientRequestHistory historyData)
        {
            if (requestData.HistoryData == null)
                requestData.HistoryData = new List<ClientRequestHistory>();
            requestData.HistoryData.Add(historyData);
        }
    }
}
