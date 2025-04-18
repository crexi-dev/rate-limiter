using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Core;
using RateLimiter.Core.Rules;
using RateLimiter.Middleware;
using RateLimiter.Examples;
using Microsoft.AspNetCore.Hosting;

namespace RateLimiter
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // register rate limiter
            services.AddSingleton<Core.RateLimiter>(RateLimiterExamples.CreateExampleRateLimiter());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // add middleware
            app.UseMiddleware<RateLimitingMiddleware>();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}