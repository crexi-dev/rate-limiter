namespace RateLimiter;

public sealed class ResourceEntry
{
    public ResourceEntry(Attempt[] attempts, int count)
    {
        Count = count;
        Attempts = attempts;
    }

    public int Count { get; private set; }
    public Attempt[] Attempts { get; }

    public ResourceEntry WithNewAttempt(Attempt attempt)
    {
        //TODO: Add Out of index check       
        Attempts[Count] = attempt; 
        Count++;

        return this;
    }
}