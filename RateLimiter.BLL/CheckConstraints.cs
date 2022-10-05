using System;
using System.Threading;
using System.Threading.Tasks;
using RateLimiter.DAL.Entities;
using RateLimiter.DAL.Entities.Interfaces;
using RateLimiter.DAL.Repositories;

namespace RateLimiter;

public class CheckConstraints : ICheckConstraints
{
    private readonly IHistoryRepository _historyRepository;
    private readonly IGenericRepository<Set> _setRepository;
    private readonly IGenericRepository<Token> _tokenRepository;

    public CheckConstraints(IHistoryRepository historyRepository, IGenericRepository<Token> tokenRepository,
        IGenericRepository<Set> setRepository)
    {
        _historyRepository = historyRepository;
        _tokenRepository = tokenRepository;
        _setRepository = setRepository;
    }

    public async Task<bool> AccessGranted(Guid tokenId, CancellationToken token)
    {
        var tokenEntity = await _tokenRepository.Read(tokenId, token);

        if (tokenEntity == null) return false;

        tokenEntity.Set = await _setRepository.Read(tokenEntity.SetId, token);
        var lastSession = await _historyRepository.GetHistoryByTokenId(tokenId, token);
        if (lastSession == null)
        {
            lastSession = new History
            {
                Id = Guid.NewGuid(),
                Count = 1,
                LastCallInUtc = DateTime.UtcNow,
                TokenId = tokenId,
                SetId = tokenEntity.SetId
            };
            await _historyRepository.Create(lastSession, token);
        }

        switch (tokenEntity.Set)
        {
            case SetByLastCall set:
            {
                var elapsed = DateTime.UtcNow - lastSession.LastCallInUtc;
                if (elapsed.TotalMilliseconds > set.SuspenseTimeInMilliseconds)
                {
                    lastSession.LastCallInUtc = DateTime.UtcNow;
                    lastSession.Set ??= set;
                    lastSession.Token = tokenEntity;
                    lastSession.Token.History = lastSession;
                    await _historyRepository.CreateOrUpdate(lastSession, token);
                    return true;
                }

                break;
            }
            case SetByCountPerTime set:
            {
                return await CheckConstrainsForLimitByCount(lastSession, set, token);
            }
        }

        return false;
    }

    private async Task ResetSession(History lastSession, SetByCountPerTime set, CancellationToken token)
    {
        lastSession.Count = 0;
        lastSession.LastCallInUtc = DateTime.UtcNow;
        lastSession.Set ??= set;
        await _historyRepository.CreateOrUpdate(lastSession, token);
    }

    private async Task<bool> CheckConstrainsForLimitByCount(History lastSession, SetByCountPerTime set,
        CancellationToken token)
    {
        var elapsed = DateTime.UtcNow - lastSession.LastCallInUtc;
        if (elapsed.TotalMilliseconds < set.PerTimeInMilliseconds)
        {
            if (lastSession.Count == set.Count)
            {
                await ResetSession(lastSession, set, token);
                return false;
            }

            lastSession.Count++;
            await _historyRepository.CreateOrUpdate(lastSession, token);
            return true;
        }

        lastSession.LastCallInUtc = DateTime.UtcNow;
        await _historyRepository.CreateOrUpdate(lastSession, token);
        // return await CheckConstrainsForLimitByCount(lastSession, set, token);
        return false;
    }
}