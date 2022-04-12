using RateLimiter.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Storage
{
    public class InMemoryStore
    {

        private static readonly ConcurrentDictionary<string, RequestDetails> _inMemoryStore  = new ConcurrentDictionary<string, RequestDetails>();
        public ConcurrentDictionary<string, RequestDetails>  AccessStorage()
        {
            return _inMemoryStore;
        }

        public void  ClearStorage()
        {
            _inMemoryStore.Clear();
        }
    }
}
