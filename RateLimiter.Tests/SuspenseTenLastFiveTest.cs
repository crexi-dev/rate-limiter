using System;
using System.Threading;
using RateLimiter.DAL.Entities;
using RateLimiter.DAL.Entities.Interfaces;
using RateLimiter.DAL.Repositories;
using Xunit;

namespace RateLimiter.Tests;

public class SuspenseTenLastFiveTest
{
    private readonly CheckConstraints _checkConstraints;

    public SuspenseTenLastFiveTest()
    {
        var repository = new GenericRepository<Token>(Setup.GetMemoryContext());
        _checkConstraints = new CheckConstraints(new HistoryRepository(Setup.GetMemoryContext()), repository,
            new GenericRepository<Set>(Setup.GetMemoryContext()));
        // 10 seconds
        repository.Create(new Token
        {
            Id = Guid.Parse("BB06989D-6A1B-4677-A4B6-AF87C0206EEA"),
            Set = new SetByLastCall
            {
                Id = Guid.Parse("77164437-B857-4C8E-B67E-26D84FD17DBF"),
                SuspenseTimeInMilliseconds = 10000
            },
            SetId = Guid.Parse("77164437-B857-4C8E-B67E-26D84FD17DBF"),
            History = new History
            {
                TokenId = Guid.Parse("BB06989D-6A1B-4677-A4B6-AF87C0206EEA"),
                LastCallInUtc = DateTime.UtcNow.AddSeconds(-5)
            }
        }, CancellationToken.None).GetAwaiter().GetResult();
    }

    [Fact]
    public void SuspenseTenLastFiveShouldBeFalse()
    {
        var assert = _checkConstraints
            .AccessGranted(Guid.Parse("BB06989D-6A1B-4677-A4B6-AF87C0206EEA"), CancellationToken.None).GetAwaiter()
            .GetResult();
        Assert.False(assert);
    }
}