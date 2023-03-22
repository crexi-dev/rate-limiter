using System;

namespace RateLimiter.Models;

public class ClientRequestKey {
    public string ClientIdentifier { get; }
    public string ResourceAccessed { get; }

    public ClientRequestKey(string clientIdentifier, string resourceAccessed) {
        ClientIdentifier = clientIdentifier;
        ResourceAccessed = resourceAccessed;
    }

    public override bool Equals(object obj) {
        if (obj is ClientRequestKey other) {
            return ClientIdentifier == other.ClientIdentifier && ResourceAccessed == other.ResourceAccessed;
        }

        return false;
    }

    public override int GetHashCode() {
        return HashCode.Combine(ClientIdentifier, ResourceAccessed);
    }
}