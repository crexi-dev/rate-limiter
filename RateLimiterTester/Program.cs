using RateLimiter;

await TestUsClient();

async Task TestUsClient()
{
    await TestClient(ClientLocation.CH);
}

async Task TestClient(ClientLocation clientLocation)
{
    var clientId = Guid.NewGuid();

    for (int i = 0; i < 1000; i++)
    {
        var request = new Request { ClientId = clientId, ClientLocation = clientLocation };

        var rateLimitExceeded = await DefaultRateLimiter.Instance.RateLimitExceeded(request);

        Console.WriteLine($"[{i}]: Rate Limit Exceeded: {rateLimitExceeded}");

        await Task.Delay(TimeSpan.FromMilliseconds(45));
    }
}