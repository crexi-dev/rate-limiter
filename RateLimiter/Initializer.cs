using System;
using System.Collections.Generic;
using RateLimiter.Rules;

namespace RateLimiter;

public static class Initializer
{
    private static readonly List<Client> Clients = new List<Client>();

    public static IEnumerable<Client> GetClients()
    {
        var client1 = new Client();

        client1.SetRateLimitRule
        (
            new XRequestPerTimeSpanRule
            (
                clientIdentifier: client1.Identifier,
                configuration: new Configuration
                {
                    Limit = 4,
                    TimeSpan = TimeSpan.FromSeconds(3)
                }
            )
        );

        var client2 = new Client();

        client2.SetRateLimitRule
        (
            new XRequestPerTimeSpanRule
            (
                clientIdentifier: client2.Identifier,
                configuration: new Configuration
                {
                    Limit = 4,
                    TimeSpan = TimeSpan.FromSeconds(3)
                }
            )
        );

        client2.SetRateLimitRule
        (
            new TimeSpanPassedSinceLastCallRule
            (
                clientIdentifier: client2.Identifier,
                configuration: new Configuration
                {
                    Limit = 4,
                    TimeSpan = TimeSpan.FromSeconds(3)
                }
            )
        );

        Clients.Add(client1);
        Clients.Add(client2);

        return Clients;
    }

    public static Request CreateRequest(Guid? clientId)
    {
        var request = new Request
        {
            CreateTime = DateTime.Now,
            ClientId = clientId ?? Guid.NewGuid(),
        };

        return request;
    }
}