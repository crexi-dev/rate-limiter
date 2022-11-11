using Example;
using RateLimiter.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<CustomHeaderSwaggerAttribute>();
});
builder.Services.AddRateLimiting()
    .AddUserProvider(context => context.Request.Headers["client-id"])
    .AddRegionProvider(context => context.Request.Headers["region"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<RateLimitingMiddleware>();

app.MapControllers();

app.Run();