using Serilog;
using WeatherForecast.WebApp.Domain.DTOs;
using WeatherForecast.WebApp.Services;

namespace WeatherForecast.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
       {
            try
            {
                var configuration = ConfigureLogger(); 

                Log.Information("Starting up the Weather Forecast application...");
                var builder = WebApplication.CreateBuilder(args);


                builder.Services.AddControllersWithViews();
                builder.Services.AddHttpClient();
                builder.Services.AddSession();
                builder.Services.AddHttpContextAccessor();
                builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();
                builder.Services.Configure<WeatherApiOptions>(configuration.GetSection("WeatherApi")); 

                var app = builder.Build();

                app.UseStaticFiles();
                app.UseRouting();
                app.UseSession();
                app.UseAuthorization();
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=WeatherForecast}/{action=WeatherCity}/{id?}");

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application startup failed.");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IConfiguration ConfigureLogger() 
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            return configuration; 
        }
    }
}
