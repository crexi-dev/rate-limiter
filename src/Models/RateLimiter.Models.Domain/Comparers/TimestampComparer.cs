using System.Collections.Generic;

namespace RateLimiter.Models.Domain.Comparers
{
    /// <summary>
    /// Customize the sort order of a collection
    /// </summary>
    /// <returns></returns>
    public class TimestampComparer : IComparer<Request>
    {
        //Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        public int Compare(Request x, Request y) => x.Timestamp.CompareTo(y.Timestamp);
    }
}
