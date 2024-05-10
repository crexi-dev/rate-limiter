using RateLimitAPI;
using RateLimiter.Classes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton(RateLimitSetup.ConfigureRateLimiting());
builder.Services.AddControllers(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var rateLimitManager = app.Services.GetRequiredService<RateLimitManager>();
    string token = context.Request.Headers["Authorization"]; // Extract token as per API design
    string path = context.Request.Path.Value;

    if (rateLimitManager.IsRequestAllowed(token, path))
    {
        await next();
    }
    else
    {
        context.Response.StatusCode = 429; // Too Many Requests
        await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
    }
});

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // Map controllers if using MVC
});

app.Run();
