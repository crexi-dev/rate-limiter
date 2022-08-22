using System.Collections.Generic;

namespace RateLimiter.DataStore
{
    public static class DataStore
    {
        public static List<RuleAStore> RuleAStores { get; } = new();
        public static List<RuleBStore> RuleBStores { get; } = new();

        public static void ClearDataStore()
        {
            RuleAStores.Clear();
            RuleBStores.Clear();
        }
    }
}