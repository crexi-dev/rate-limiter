using RateLimiter;
using RateLimiterApi;
using RateLimiterApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.OperationFilter<CustomHeaderSwaggerAttribute>();
});
builder.Services.AddMemoryCache();

builder.Services.ConfigureRateLimiter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseMiddleware<RateLimitMiddleware>();
app.MapControllers();

app.Run();

public partial class Program { }