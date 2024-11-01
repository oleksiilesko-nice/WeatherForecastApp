using WeatherForecast.WebApp.Models;

namespace WeatherForecast.WebApp.Services
{
    public interface IWeatherForecastService
    {
        Task<WeatherForecastModel> GetWeatherDataAsync(string cityName);
        bool IsRainWarningShown(string cityName);
        void SetRainWarningCookie(string cityName);
    }

}
