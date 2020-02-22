using System.Collections.Concurrent;

namespace RateLimiter.State
{
    public class InMemoryRuleState : IRulePersist
    {
        private readonly ConcurrentDictionary<string, object> _keyValues;
        private static readonly object Lock = new object();
        private static InMemoryRuleState instance = null;

        private InMemoryRuleState()
        {
            _keyValues = new ConcurrentDictionary<string, object>();
        }

        public static InMemoryRuleState GetInstance
        {
            get
            {
                lock (Lock)
                {
                    if (instance == null)
                    {
                        instance = new InMemoryRuleState();
                    }
                    return instance;
                }
            }
        }

        public bool Put<T>(string key, T value)
        {
            if (_keyValues.TryGetValue(key, out var val))
            {
                return _keyValues.TryUpdate(key, value, val);
            }

            return _keyValues.TryAdd(key, value);
        }

        public (T,bool) Retrieve<T>(string key)
        {
            if (_keyValues.TryGetValue(key, out var val))
                return ((T)val, true);

            return (default, false);
        }
    }
}