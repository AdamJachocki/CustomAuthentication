using WeatherApi.Authentication;
using WeatherApi.Data;

namespace WeatherApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddAuthentication("ApiKeyOrBasic")
                .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>("ApiKey", o =>
                {
                    o.ApiKeyLength = 7;
                    o.CheckApiKeyLength = true;
                })
                .AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>("Basic", null)
                .AddPolicyScheme("ApiKeyOrBasic", null, o =>
                {
                    o.ForwardDefaultSelector = context =>
                    {
                        if (context.Request.Headers.ContainsKey("X-API-KEY"))
                            return "ApiKey";
                        else
                            return "Basic";
                    };
                });

            builder.Services.AddScoped<ApiKeyClientProvider>();
            builder.Services.AddScoped<UserProvider>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}