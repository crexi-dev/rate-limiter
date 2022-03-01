using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    public interface IResource
    {
        IEnumerable<IRule> Rules { get; init; }
        IJournal Journal { get; init; }
        IService Service { get; init; }
        Task Execute(string token);
    }
}