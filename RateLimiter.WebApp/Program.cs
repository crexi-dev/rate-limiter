using RateLimiter.Machine;
using RateLimiter.Models;
using RateLimiter.Rules;
using RateLimiter.WebApp.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IRuleStateMachine<RequestData>>(provider =>
{
    var rules = new[]
    {
        new RequestsPerTimespanRule(TimeSpan.FromMinutes(1), 2),
        //a certain timespan passed since the last call for EU started tokens
        new RequestsPerTimespanTokenBasedRule(TimeSpan.FromSeconds(5), 1, "EU.*"),
        new RequestsPerTimespanTokenBasedRule(TimeSpan.FromSeconds(10), 3, "US.*"),
    };
    return RuleStateMachineBuilder.Build(rules, 
        LoggerFactory.Create(loggingBuilder => loggingBuilder.AddSimpleConsole())
            .CreateLogger<IRuleStateMachine<RequestData>>());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

//app.UseAuthorization();

/*
 Request cURL example:
 curl -X 'GET' \
  'http://localhost:5252/WeatherForecast' \
  -H 'accept: text/plain' \
  --cookie token=test
 */
app.UseRateLimit();

app.MapControllers();

app.Run();