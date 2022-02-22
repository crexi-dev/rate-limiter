using System.Collections.Generic;

namespace RateLimits.History
{
    public interface IHistoryRepository
    {
        List<HistoryModel> Get(string accessToken, string resource);
        void Add(string accessToken, string resource, string region);

    }
}
