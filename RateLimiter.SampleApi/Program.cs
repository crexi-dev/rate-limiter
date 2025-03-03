using RateLimiter;
using RateLimiter.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

builder.Services.
    AddFixedWindow(configure =>
    {
        configure.RuleConditions = new List<Func<AccessToken, bool>>
        {
            token => token.Region == Region.us,
            token => !string.IsNullOrEmpty(token.UserId)
        };

        configure.Limit = 10;
        configure.WindowSize = TimeSpan.FromMinutes(2);
    }).
    AddTimeBasedRateLimiting(configure =>
    {
        configure.RuleConditions = new List<Func<AccessToken, bool>>
        {
            token => token.Region == Region.us || token.Region == Region.eu,
            token => !string.IsNullOrEmpty(token.UserId)
        };

        configure.MinTimeBetweenRequests = TimeSpan.FromSeconds(10);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

