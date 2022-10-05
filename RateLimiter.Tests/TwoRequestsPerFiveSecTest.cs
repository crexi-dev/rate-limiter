using System;
using System.Threading;
using RateLimiter.DAL.Entities;
using RateLimiter.DAL.Entities.Interfaces;
using RateLimiter.DAL.Repositories;
using Xunit;

namespace RateLimiter.Tests;

public class TwoRequestsPerFiveSecTest
{
    private readonly CheckConstraints _checkConstraints;

    public TwoRequestsPerFiveSecTest()
    {
        var repository = new GenericRepository<Token>(Setup.GetMemoryContext());
        _checkConstraints = new CheckConstraints(new HistoryRepository(Setup.GetMemoryContext()), repository,
            new GenericRepository<Set>(Setup.GetMemoryContext()));
        repository.Create(new Token
        {
            Id = Guid.Parse("47A9D3A5-E270-43A7-8FA4-8FFAF406A441"),
            Set = new SetByCountPerTime
            {
                Id = Guid.Parse("7CBFCBE1-4942-437D-B97A-219831B3AD8F"),
                Count = 2,
                PerTimeInMilliseconds = 5000
            },
            SetId = Guid.Parse("7CBFCBE1-4942-437D-B97A-219831B3AD8F"),
            History = new History
            {
                Id = Guid.Parse("08CD2834-8A3C-48A0-96CA-30395FB07635"),
                TokenId = Guid.Parse("47A9D3A5-E270-43A7-8FA4-8FFAF406A441"),
                Count = 0,
                LastCallInUtc = DateTime.UtcNow
            }
        }, CancellationToken.None).GetAwaiter().GetResult();
    }

    [Fact]
    public void TwoRequestsPerFiveSecShouldBeTrue()
    {
        var assert = _checkConstraints
            .AccessGranted(Guid.Parse("47A9D3A5-E270-43A7-8FA4-8FFAF406A441"), CancellationToken.None).GetAwaiter()
            .GetResult();
        Assert.True(assert);

        assert = _checkConstraints
            .AccessGranted(Guid.Parse("47A9D3A5-E270-43A7-8FA4-8FFAF406A441"), CancellationToken.None).GetAwaiter()
            .GetResult();
        Assert.True(assert);

        assert = _checkConstraints
            .AccessGranted(Guid.Parse("47A9D3A5-E270-43A7-8FA4-8FFAF406A441"), CancellationToken.None).GetAwaiter()
            .GetResult();
        Assert.False(assert);
    }
}