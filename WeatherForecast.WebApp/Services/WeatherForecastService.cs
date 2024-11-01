using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WeatherForecast.WebApp.Domain.DTOs;
using WeatherForecast.WebApp.Models;

namespace WeatherForecast.WebApp.Services
{
    public class WeatherForecastService : IWeatherForecastService
    {
        private readonly HttpClient _httpClient;
        private readonly WeatherApiOptions _weatherApiOptions;
        private readonly ILogger<WeatherForecastService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WeatherForecastService(HttpClient httpClient, IOptions<WeatherApiOptions> weatherApiOptions, ILogger<WeatherForecastService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _weatherApiOptions = weatherApiOptions.Value;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<WeatherForecastModel> GetWeatherDataAsync(string cityName)
        {
            try
            {
                _logger.LogInformation("Making API call to get weather data for city: {CityName}", cityName);
                var url = $"{_weatherApiOptions.BaseUrl}weather?q={cityName}&units=metric&appid={_weatherApiOptions.ApiKey}";
                var response = await _httpClient.GetStringAsync(url);
                var weatherApiResponse = JsonConvert.DeserializeObject<WeatherApiResponse>(response);

                var weatherForecast = MapToWeatherForecastModel(weatherApiResponse);

                _logger.LogInformation("Weather data retrieved successfully for city: {CityName}", cityName);
                return weatherForecast;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve weather data for city: {CityName}", cityName);
                throw;
            }
        }

        public bool IsRainWarningShown(string cityName)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            return request.Cookies.TryGetValue($"RainWarning_{cityName}", out var cookieDate)
                   && DateTime.TryParse(cookieDate, out var lastShownDate)
                   && lastShownDate.Date == DateTime.UtcNow.Date;
        }

        public void SetRainWarningCookie(string cityName)
        {
            var response = _httpContextAccessor.HttpContext.Response;
            response.Cookies.Append(
                $"RainWarning_{cityName}",
                DateTime.UtcNow.Date.ToString("yyyy-MM-dd"),
                new CookieOptions { Expires = DateTime.UtcNow.AddDays(1) }
            );
        }

        private static WeatherForecastModel MapToWeatherForecastModel(WeatherApiResponse weatherApiResponse)
        {
            return new WeatherForecastModel
            {
                CityName = weatherApiResponse.Name,
                Temperature = weatherApiResponse.Main?.Temp ?? 0,
                FeelsLike = weatherApiResponse.Main?.FeelsLike ?? 0,
                MinTemperature = weatherApiResponse.Main?.TempMin ?? 0,
                MaxTemperature = weatherApiResponse.Main?.TempMax ?? 0,
                Pressure = weatherApiResponse.Main?.Pressure ?? 0,
                Humidity = weatherApiResponse.Main?.Humidity ?? 0,
                SeaLevel = weatherApiResponse.Main?.SeaLevel ?? 0,
                GrndLevel = weatherApiResponse.Main?.GrndLevel ?? 0,
                Visibility = weatherApiResponse.Visibility ?? 0,
                WindSpeed = weatherApiResponse.Wind?.Speed ?? 0,
                WindDirection = weatherApiResponse.Wind?.Deg ?? 0,
                CloudCoverage = weatherApiResponse.Clouds?.All ?? 0,
                Condition = weatherApiResponse.Weather?.FirstOrDefault()?.Description ?? string.Empty,
                Country = weatherApiResponse.Sys?.Country ?? string.Empty,
                Sunrise = weatherApiResponse.Sys?.Sunrise != null ? DateTimeOffset.FromUnixTimeSeconds(weatherApiResponse.Sys.Sunrise.Value).DateTime : DateTime.MinValue,
                Sunset = weatherApiResponse.Sys?.Sunset != null ? DateTimeOffset.FromUnixTimeSeconds(weatherApiResponse.Sys.Sunset.Value).DateTime : DateTime.MinValue,
                Latitude = weatherApiResponse.Coord?.Lat ?? 0,
                Longitude = weatherApiResponse.Coord?.Lon ?? 0,
                LastUsedCity = weatherApiResponse.Name,
                IsRainExpected = weatherApiResponse.Weather?.Any(w => w.Main?.ToLower() == "rain") ?? false
            };
        }
    }
}
