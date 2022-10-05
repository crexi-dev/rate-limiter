using System;
using System.Threading;
using RateLimiter.DAL.Entities;
using RateLimiter.DAL.Entities.Interfaces;
using RateLimiter.DAL.Repositories;
using Xunit;
using Xunit.Sdk;

namespace RateLimiter.Tests;

public class ThousandRequestsPerOneSecTest
{
    private readonly CheckConstraints _checkConstraints;

    public ThousandRequestsPerOneSecTest()
    {
        var repository = new GenericRepository<Token>(Setup.GetMemoryContext());
        _checkConstraints = new CheckConstraints(new HistoryRepository(Setup.GetMemoryContext()), repository,
            new GenericRepository<Set>(Setup.GetMemoryContext()));
        repository.Create(new Token
        {
            Id = Guid.Parse("9242B382-94F1-490E-8347-61D5810A908A"),
            Set = new SetByCountPerTime
            {
                Id = Guid.Parse("3C6B9CA5-50A4-4DFD-A260-9378401B8268"),
                Count = 1000,
                PerTimeInMilliseconds = 850
            },
            SetId = Guid.Parse("3C6B9CA5-50A4-4DFD-A260-9378401B8268"),
            History = new History
            {
                Id = Guid.Parse("0AAC9170-5BB3-43A4-829B-50D49206312E"),
                TokenId = Guid.Parse("9242B382-94F1-490E-8347-61D5810A908A"),
                Count = 0,
                LastCallInUtc = DateTime.UtcNow
            }
        }, CancellationToken.None).GetAwaiter().GetResult();
    }

    [Fact]
    public void ThousandRequestsPerOneSecShouldBeThrow()
    {
        Assert.Throws<TrueException>(() =>
        {
            for (var i = 1; i <= 1000; i++)
                Assert.True(_checkConstraints
                    .AccessGranted(Guid.Parse("9242B382-94F1-490E-8347-61D5810A908A"), CancellationToken.None)
                    .GetAwaiter()
                    .GetResult());
        });
    }
}