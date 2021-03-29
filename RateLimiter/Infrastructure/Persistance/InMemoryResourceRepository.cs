using RateLimiter.Domain.Aggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Infastructure.Persistance
{
    public class InMemoryResourceRepository : IResourceRepository
    {
        private readonly Dictionary<string, Resource> _data;

        public InMemoryResourceRepository(Dictionary<string, Resource> data = null)
        {
            _data = data ?? new Dictionary<string, Resource>();
        }

        public Resource Get(string key)
        {
            if(_data.ContainsKey(key))
            {
                return _data[key];
            }

            throw new KeyNotFoundException();
        }
        
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
