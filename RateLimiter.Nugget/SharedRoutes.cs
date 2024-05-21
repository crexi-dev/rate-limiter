using System.Collections.ObjectModel;

namespace RateLimiter.Nugget
{
    public static class SharedRoutes
    {
        private static Dictionary<string, List<DateTime>> _requestInfo = new();
        private static Dictionary<Type, IReadOnlyList<string>> _routes = new();

        public static IReadOnlyDictionary<Type, IReadOnlyList<string>> GetRoutes()
            => new ReadOnlyDictionary<Type, IReadOnlyList<string>>(_routes);

        public static void AddRoutes<T>(List<string> newRoutes)
            => _routes.Add(typeof(T), newRoutes);

        public static List<Type> GetLimiterRuleTypeByRoute(string value)
        {
            return _routes
                .Where(pair => pair.Value.Contains(value))
                .Select(pair => pair.Key)
                .ToList();
        }

        // Create
        public static void AddRequestInfo(string key, DateTime value)
        {
            if (_requestInfo.ContainsKey(key))
            {
                _requestInfo[key].Add(value);
            }
            else
            {
                _requestInfo[key] = new List<DateTime> { value };
            }
        }

        // Read
        public static List<DateTime> GetRequestInfo(string key)
        {
            if (_requestInfo.ContainsKey(key))
            {
                return _requestInfo[key];
            }
            else
            {
                return null;
            }
        }

        // Update
        public static void UpdateRequestInfo(string key, List<DateTime> values)
        {
            if (_requestInfo.ContainsKey(key))
            {
                _requestInfo[key] = values;
            }
            else
            {
                throw new KeyNotFoundException($"No such key: {key}");
            }
        }

        // Delete
        public static void DeleteRequestInfo(string key)
        {
            if (_requestInfo.ContainsKey(key))
            {
                _requestInfo.Remove(key);
            }
            else
            {
                throw new KeyNotFoundException($"No such key: {key}");
            }
        }
    }
}
