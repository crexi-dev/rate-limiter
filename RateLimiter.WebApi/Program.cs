using RateLimiter.FixedCapacityByCountryPolicy;
using RateLimiter.RequestLimiterPolicy;

namespace RateLimiter.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddMemoryCache();
            builder.Services.Configure<RequestLimiterMiddleWareOptions>(builder.Configuration.GetSection("RequestLimiterPolicy"));
            builder.Services.Configure<FixedCapacityByCountryMiddleWareOptions >(builder.Configuration.GetSection("FixedCapacityByCountryPolicy"));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseRequestLimiterPolicy();
            app.UseFixedCapacityPolicy();
            app.UseFixedCapacityByCountryPolicy();

            app.MapControllers();

            app.Run();
        }
    }
}
