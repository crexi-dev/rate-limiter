using RateLimiter;
using RateLimiter.Models.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMemoryCache();
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddControllers();

builder.Services.Configure<ActiveProcessorsOptions>(builder.Configuration.GetSection(nameof(ActiveProcessorsOptions)));
builder.Services.Configure<LastCallTimeSpanOptions>(builder.Configuration.GetSection(nameof(LastCallTimeSpanOptions)));
builder.Services.Configure<RequestRateOptions>(builder.Configuration.GetSection(nameof(RequestRateOptions)));

var app = builder.Build();

app.UseClientRateLimiting();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
