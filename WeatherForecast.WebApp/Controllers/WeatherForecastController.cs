using Microsoft.AspNetCore.Mvc;
using WeatherForecast.WebApp.Models;
using WeatherForecast.WebApp.Services;

public class WeatherForecastController : Controller
{
    private readonly IWeatherForecastService _weatherForecastService;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(IWeatherForecastService weatherForecastService, ILogger<WeatherForecastController> logger)
    {
        _weatherForecastService = weatherForecastService;
        _logger = logger;
    }

    public IActionResult WeatherCity()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GetWeather(string cityName)
    {
        try
        {
            _logger.LogInformation("Fetching weather data for city: {CityName}", cityName);
            WeatherForecastModel weatherData = await _weatherForecastService.GetWeatherDataAsync(cityName);
            _logger.LogInformation("Weather data retrieved successfully for city: {CityName}", cityName);

            if (weatherData.IsRainExpected && !_weatherForecastService.IsRainWarningShown(cityName))
            {
                ViewBag.RainWarning = $"Warning: Rain is expected in {cityName} today!";
                _weatherForecastService.SetRainWarningCookie(cityName);
            }

            return View("WeatherResult", weatherData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to retrieve weather data for city: {CityName}", cityName);
            ModelState.AddModelError("", "Unable to retrieve weather data. Please try again later.");
            return View("WeatherCity");
        }
    }

}
