using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.ClientStatistics;

public class CombineBucketIdentifier : IBucketIdentifier
{
    private readonly List<IBucketIdentifier> _ids;

    public CombineBucketIdentifier()
    {
        _ids = new List<IBucketIdentifier>();
    }
    
    public void Add(IBucketIdentifier id)
    {
        _ids.Add(id);
    }

    public override int GetHashCode()
    {
        return _ids.Aggregate(0, (res, item) => res ^ item.GetHashCode());
    }

    public override bool Equals(object? obj)
    {
        return obj is CombineBucketIdentifier id && Equals(id);
    }
    
    private bool Equals(CombineBucketIdentifier obj)
    {
        if (_ids.Count != obj._ids.Count) return false;
        for (int i = _ids.Count - 1; i >= 0; i--)
        {
            if (!_ids[i].Equals(obj._ids[i]))
                return false;
        }
        
        return true;
    }
}