using System;

namespace RateLimiter.ClientStatistics;

internal class BucketIdentifier<T> :  IBucketIdentifier
{
    private readonly T _id;
    
    public BucketIdentifier(T id)
    {
        _id = id;
    }
    
    public override int GetHashCode()
    {
        if (_id != null) return _id.GetHashCode();
        throw new NullReferenceException();
    }

    public override bool Equals(object? obj)
    {
        return obj is BucketIdentifier<T> id && Equals(id);
    }
    
    public bool Equals(BucketIdentifier<T> obj)
    {
        return _id != null && _id.Equals(obj._id);
    }
}