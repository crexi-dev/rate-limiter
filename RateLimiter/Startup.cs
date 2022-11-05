using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.RateLimitRules;
using RateLimiter.Repository;
using RateLimiter.Settings;

namespace RateLimiter
{
    public class Startup
    {

        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMemoryCache();

            services.AddSingleton<IRulesConfigService, RulesConfigService>();
            services.AddSingleton<IRulesProcessorService, RulesProcessorService>();

            services.AddSingleton<IRulesRepository, RulesRepository>();
            services.AddSingleton<IEventsRepository, EventsRepository>();

            services.Configure<RateLimiterRules>(Configuration.GetSection("RateLimiterRules"));

            services.AddHostedService<WorkerService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {

            app.UseResourceLimiterMiddleware();


            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }


    }
}
