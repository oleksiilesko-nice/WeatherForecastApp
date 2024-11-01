using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherForecast.WebApp.Models;
using WeatherForecast.WebApp.Services;

namespace WeatherForecast.Tests
{
    public class WeatherForecastControllerTests
    {
        private readonly Mock<IWeatherForecastService> _weatherForecastServiceMock;
        private readonly Mock<ILogger<WeatherForecastController>> _loggerMock;
        private readonly WeatherForecastController _controller;

        public WeatherForecastControllerTests()
        {
            _weatherForecastServiceMock = new Mock<IWeatherForecastService>();
            _loggerMock = new Mock<ILogger<WeatherForecastController>>();
            _controller = new WeatherForecastController(_weatherForecastServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetWeather_ReturnsViewResult_WithWeatherData()
        {
            string cityName = "London";
            var weatherData = new WeatherForecastModel
            {
                CityName = cityName,
                Temperature = 20,
                IsRainExpected = false
            };

            _weatherForecastServiceMock.Setup(s => s.GetWeatherDataAsync(cityName))
                .ReturnsAsync(weatherData);

            var result = await _controller.GetWeather(cityName) as ViewResult;

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task GetWeather_ShowsRainWarning_WhenRainIsExpected()
        {
            string cityName = "London";
            var weatherData = new WeatherForecastModel
            {
                CityName = cityName,
                Temperature = 20,
                IsRainExpected = true
            };

            _weatherForecastServiceMock.Setup(s => s.GetWeatherDataAsync(cityName))
                .ReturnsAsync(weatherData);
            _weatherForecastServiceMock.Setup(s => s.IsRainWarningShown(cityName))
                .Returns(false); 

            var result = await _controller.GetWeather(cityName) as ViewResult;

            Assert.Equal($"Warning: Rain is expected in {cityName} today!", result.ViewData["RainWarning"]);
        }


        [Fact]
        public async Task GetWeather_ReturnsWeatherCityView_WhenExceptionOccurs()
        {
            string cityName = "London";
            _weatherForecastServiceMock.Setup(s => s.GetWeatherDataAsync(cityName))
                .ThrowsAsync(new Exception("API call failed"));

            var result = await _controller.GetWeather(cityName) as ViewResult;

            Assert.Equal("WeatherCity", result.ViewName);
        }
        [Fact]
        public async Task GetWeather_ReturnsWeatherResult_WhenDataIsRetrievedSuccessfully()
        {
            string cityName = "London";
            var weatherData = new WeatherForecastModel
            {
                CityName = cityName,
                Temperature = 20,
                IsRainExpected = false
            };

            _weatherForecastServiceMock.Setup(s => s.GetWeatherDataAsync(cityName))
                .ReturnsAsync(weatherData);

            var result = await _controller.GetWeather(cityName) as ViewResult;

            Assert.Equal(weatherData, result.Model);
        }
        [Fact]
        public async Task GetWeather_AddsErrorToModelState_WhenErrorOccurs()
        {
            string cityName = "London";

            _weatherForecastServiceMock.Setup(s => s.GetWeatherDataAsync(cityName))
                .ThrowsAsync(new Exception("API call failed"));

            var result = await _controller.GetWeather(cityName) as ViewResult;

            Assert.True(_controller.ModelState.ContainsKey(""));
        }
        [Fact]
        public void WeatherCity_ReturnsViewResult()
        {
            var result = _controller.WeatherCity() as ViewResult;

            Assert.IsType<ViewResult>(result); 
        }

    }
}
