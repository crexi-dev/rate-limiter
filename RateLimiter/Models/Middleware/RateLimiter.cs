using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using RateLimiter.Constants;
using RateLimiter.Models.Config;
using RateLimiter.Models.Config.Base;

namespace RateLimiter.Models.Middleware;

public class RateLimiter
{
    // Not good caching because we don't clear it. Maybe, we need some async job that will clear such resources.
    private static readonly ConcurrentDictionary<ClientInfo, ClientStatus> ClientsStatuses = new();

    private readonly RequestDelegate _next;
    private readonly RateLimitConfiguration _configuration;
    private readonly SemaphoreSlim _semaphore;

    // Would be nice to move rule groups validations to separate Processors
    private readonly bool _ipLimitEnabled;
    private readonly bool _clientKeyLimitEnabled;

    private record ClientInfo(string? IpAddress, string? Key, string Resource);
    private record ClientStatus(DateTime? AccessDate, int Calls);

    public RateLimiter(RequestDelegate next, RateLimitConfiguration config)
    {
        _next = next;
        _configuration = config;
        _ipLimitEnabled = config.IpConfiguration != null;
        _clientKeyLimitEnabled = config.ClientKeyConfiguration != null;
        _semaphore = new SemaphoreSlim(1);
    }

    public async Task Invoke(HttpContext context)
    {
        HttpResponse? response = context.Response;
        ClientInfo client = BuildClient(context);
        var doesClientExist = ClientsStatuses.TryGetValue(client, out ClientStatus? clientStatus);
        clientStatus ??= new ClientStatus(DateTime.Now, 1);
        clientStatus = await CacheClientStatus(doesClientExist, client, clientStatus);
        var allowed = IsAllowed(client);
        if (!allowed)
        {
            await WriteResponse(response, "Client is not allowed.", HttpStatusCode.Unauthorized);
            return;
        }

        if (!IsClientValid(DoesClientAdheresPolicy, clientStatus, client.Resource))
        {
            await WriteResponse(response, "Client exceeded quota.", HttpStatusCode.TooManyRequests);
            return;
        }

        RemoveClientStatus(client, clientStatus);
        await _next(context);
    }

    private async Task<ClientStatus> CacheClientStatus(bool doesClientExist, ClientInfo client, ClientStatus clientStatus)
    {
        await _semaphore.WaitAsync();
        if (doesClientExist)
        {
            clientStatus = UpdateClientStatus(client, clientStatus);
        }
        else
        {
            if (!ClientsStatuses.TryAdd(client, clientStatus))
            {
                clientStatus = UpdateClientStatus(client, clientStatus);
            }
        }
        _semaphore.Release();

        return clientStatus;
    }

    private ClientStatus UpdateClientStatus(ClientInfo info, ClientStatus status)
    {
        var clientValid = IsClientValid(IsTimeoutPeriodActive, status, info.Resource);
        ClientStatus currentValue = ClientsStatuses[info];
        if (clientValid || currentValue.AccessDate > status.AccessDate)
        {
            ClientStatus newValue = currentValue with { Calls = currentValue.Calls + 1 };
            ClientsStatuses.TryUpdate(info, newValue, currentValue);
            return newValue;
        }

        return currentValue;
    }

    private void RemoveClientStatus(ClientInfo info, ClientStatus status)
    {
        var clientValid = IsClientValid(IsTimeoutPeriodActive, status, info.Resource);
        ClientStatus currentValue = ClientsStatuses[info];
        if (!clientValid && currentValue.AccessDate == status.AccessDate)
        {
            ClientsStatuses.TryRemove(info, out _);
        }
    }

    private bool IsClientValid(Func<DateTime, Policy, ClientStatus, bool> validation, ClientStatus clientStatus,
        string path)
    {
        if (_ipLimitEnabled && DoesClientCorrespondsPolicies(validation, _configuration.IpConfiguration!, clientStatus, path))
        {
            return true;
        }

        return _clientKeyLimitEnabled && DoesClientCorrespondsPolicies(validation, _configuration.ClientKeyConfiguration!,
            clientStatus, path);
    }

    private static bool DoesClientCorrespondsPolicies(Func<DateTime, Policy, ClientStatus, bool> validation,
        BaseConfiguration configuration, ClientStatus status, string path)
    {
        if (configuration.Policies == null)
        {
            return true;
        }

        IEnumerable<Policy> pathPolicies = configuration.Policies.Where(policy => DoesPathFollowTemplate(path, policy));
        DateTime now = DateTime.Now;
        return pathPolicies.All(policy => validation(now, policy, status));
    }

    private ClientInfo BuildClient(HttpContext context)
    {
        HttpRequest request = context.Request;

        // Better to use context.Connection.RemoteIpAddress but it is more complicated to test. I believe for the test task I can use simple solution
        var ip = _ipLimitEnabled ? request.Host.ToString() : null;

        string? key = null;
        if (_clientKeyLimitEnabled)
        {
            request.Headers.TryGetValue(Headers.ClientKey, out StringValues value);
            key = value;
        }
        return new ClientInfo(ip, key, request.Path);
    }

    private bool IsAllowed(ClientInfo clientInfo)
    {
        var allowed = true;
        var (ipAddress, key, _) = clientInfo;
        if (_ipLimitEnabled)
        {
            allowed = IsInAllowedList(_configuration.IpConfiguration!, ipAddress);
        }
        if (allowed && _clientKeyLimitEnabled)
        {
            allowed = IsInAllowedList(_configuration.ClientKeyConfiguration!, key);
        }

        return allowed;
    }

    private static bool DoesPathFollowTemplate(string path, Policy policy)
    {
        return policy.Path == SpecialSymbols.Any
               || string.Compare(policy.Path, path, StringComparison.CurrentCultureIgnoreCase) == 0;
    }

    private static Task WriteResponse(HttpResponse response, string message, HttpStatusCode code)
    {
        response.StatusCode = (int)code;
        return response.WriteAsync(message);
    }

    private static bool IsInAllowedList(IAllowedClientsConfig config, string? value)
    {
        return config.AllowedIdentifiers == null || config.AllowedIdentifiers.Contains(value);
    }

    private static bool DoesClientAdheresPolicy(DateTime date, Policy policy, ClientStatus status)
    {
        if (IsTimeoutPeriodActive(date, policy, status))
        {
            return status.Calls <= policy.Count;
        }

        return true;
    }

    private static bool IsTimeoutPeriodActive(DateTime date, Policy policy, ClientStatus status)
    {
        return date < status.AccessDate!.Value.Add(policy.Timeout);
    }
}