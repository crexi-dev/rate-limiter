using AspNetCoreRateLimit;
using AspNetCoreRateLimit.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RateLimiter.Repository.Context;
using RateLimiter.Repository.Interfaces;
using RateLimiter.Repository.Repository;
using RateLimiter.Service.Interfaces;
using RateLimiter.Service.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimiter.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //In Memory Db Context configuration            
            services.AddDbContext<RateLimiterContext>(opt => opt.UseInMemoryDatabase("RateLimiterDB"));

            services.AddMvcCore();
            // Custom Repository and serivce configruation
            services.AddTransient<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IDepartmentService, DepartmentService>();

            // Initialization of : Limit request process.

            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));

            services.Configure<ClientRateLimitOptions>(Configuration.GetSection("ClientRateLimiting"));
            services.Configure<ClientRateLimitPolicies>(Configuration.GetSection("ClientRateLimitPolicies"));
 
            services.AddInMemoryRateLimiting();
            // configure the resolvers
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            // Swagger Initialization 
            services.AddSwaggerGen(sw =>
            {
                sw.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "RateLimiter API",
                    Version = "v1",
                    Description = "RateLimiter API poc.",
                    Contact = new OpenApiContact
                    {
                        Name = "Ignacio Mariscal",
                        Email = "mariscal.ignacio@gmail.com",
                        Url = new Uri("https://www.linkedin.com/in/ignacio-mariscal-martinez-31752246/"),
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "RateLimiter API v1");     
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseIpRateLimiting();
            app.UseClientRateLimiting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
