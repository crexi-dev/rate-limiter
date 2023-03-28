using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter;

public static class RequestsBucket
{
    private static readonly ICollection<Request> Requests = new List<Request>();

    public static void Add(Request request)
    {
        Requests.Add(request);
    }

    public static void Remove(Request request)
    {
        Requests.Remove(request);
    }

    public static int Count(Guid? clientId) => clientId.HasValue 
                                                            ? Requests.Count(r => r.ClientId == clientId) 
                                                            : Requests.Count;

    public static DateTime? FirstCallDateTime(Guid? clientId) => Count(clientId) == 0 
                                                            ? default 
                                                            : Requests.FirstOrDefault(r => r.ClientId == clientId)?.CreateTime;

    public static DateTime? LastCallDateTime(Guid? clientId) => Count(clientId) == 0 
                                                            ? default 
                                                            : Requests.LastOrDefault(r => r.ClientId == clientId)?.CreateTime;

    public static void Clear() => Requests.Clear();
}