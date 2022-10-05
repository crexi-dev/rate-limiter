using System;
using System.Threading;
using RateLimiter.DAL.Entities;
using RateLimiter.DAL.Entities.Interfaces;
using RateLimiter.DAL.Repositories;
using Xunit;

namespace RateLimiter.Tests;

public class SuspenseFiveLastTenTest
{
    private readonly CheckConstraints _checkConstraints;

    public SuspenseFiveLastTenTest()
    {
        var repository = new GenericRepository<Token>(Setup.GetMemoryContext());
        _checkConstraints = new CheckConstraints(new HistoryRepository(Setup.GetMemoryContext()), repository,
            new GenericRepository<Set>(Setup.GetMemoryContext()));
        // 5 seconds
        repository.Create(new Token
        {
            Id = Guid.Parse("9ED6CD54-C3DA-4BD9-8806-4FDC7F4B9EB6"),
            Set = new SetByLastCall
            {
                Id = Guid.Parse("D7547CDC-0A40-4D3B-803A-DB34C71411AD"),
                SuspenseTimeInMilliseconds = 5000
            },
            SetId = Guid.Parse("D7547CDC-0A40-4D3B-803A-DB34C71411AD"),
            History = new History
            {
                TokenId = Guid.Parse("9ED6CD54-C3DA-4BD9-8806-4FDC7F4B9EB6"),
                LastCallInUtc = DateTime.UtcNow.AddSeconds(-5)
            }
        }, CancellationToken.None).GetAwaiter().GetResult();
    }


    [Fact]
    public void SuspenseFiveLastTenShouldBeTrue()
    {
        var assert = _checkConstraints
            .AccessGranted(Guid.Parse("9ED6CD54-C3DA-4BD9-8806-4FDC7F4B9EB6"), CancellationToken.None).GetAwaiter()
            .GetResult();
        Assert.True(assert);
    }
}