using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using WeatherForecast.WebApp.Domain.DTOs;
using WeatherForecast.WebApp.Services;

namespace WeatherForecast.Tests
{
    public class WeatherForecastServiceTests
    {
        private readonly Mock<IOptions<WeatherApiOptions>> _optionsMock;
        private readonly Mock<ILogger<WeatherForecastService>> _loggerMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private  WeatherForecastService _service;
        private readonly WeatherApiOptions _weatherApiOptions;

        public WeatherForecastServiceTests()
        {
            _optionsMock = new Mock<IOptions<WeatherApiOptions>>();
            _loggerMock = new Mock<ILogger<WeatherForecastService>>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _weatherApiOptions = new WeatherApiOptions
            {
                ApiKey = "testApiKey",
                BaseUrl = "https://api.openweathermap.org/"
            };

            _optionsMock.Setup(o => o.Value).Returns(_weatherApiOptions);

            // Создание мока HttpClient
            var mockHttp = new MockHttpMessageHandler();
            _service = new WeatherForecastService(
                mockHttp.ToHttpClient(), 
                _optionsMock.Object,
                _loggerMock.Object,
                _httpContextAccessorMock.Object);
        }

        [Fact]
        public async Task GetWeatherDataAsync_ReturnsWeatherForecastModel()
        {
            // Arrange
            var cityName = "London";
            var jsonResponse = JsonConvert.SerializeObject(new WeatherApiResponse
            {
                Name = cityName,
                Main = new Main { Temp = 15.0, FeelsLike = 14.0, TempMin = 10.0, TempMax = 20.0, Pressure = 1012, Humidity = 80 },
                Wind = new Wind { Speed = 5.0, Deg = 180 },
                Visibility = 10000,
                Weather = new List<Weather> { new Weather { Main = "Rain", Description = "light rain" } },
                Sys = new Sys { Country = "GB", Sunrise = 1634300800, Sunset = 1634344000 },
                Coord = new Coord { Lat = 51.5074, Lon = -0.1278 }
            });

            // Настройка мок HTTP-запроса
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Get, $"https://api.openweathermap.org/weather?q={cityName}&units=metric&appid={_weatherApiOptions.ApiKey}")
                    .Respond("application/json", jsonResponse);

            // Используем мок HTTP-клиент
            var httpClient = mockHttp.ToHttpClient();
            _service = new WeatherForecastService(httpClient, _optionsMock.Object, _loggerMock.Object, _httpContextAccessorMock.Object);

            // Act
            var result = await _service.GetWeatherDataAsync(cityName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cityName, result.CityName);
        }
        [Fact]
        public void IsRainWarningShown_ReturnsTrue_WhenCookieExists()
        {
            var cityName = "London";
            var cookieKey = $"RainWarning_{cityName}";
            var cookieValue = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");

            var context = new DefaultHttpContext();

            var cookiesMock = new Mock<IRequestCookieCollection>();
            cookiesMock.Setup(c => c.TryGetValue(cookieKey, out cookieValue)).Returns(true);

            context.Request.Cookies = cookiesMock.Object;

            _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(context);

            var result = _service.IsRainWarningShown(cityName);

            Assert.True(result);
        }



        [Fact]
        public void IsRainWarningShown_ReturnsFalse_WhenCookieDoesNotExist()
        {
            var cityName = "London";
            var context = new DefaultHttpContext();
            _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(context);

            var result = _service.IsRainWarningShown(cityName);

            Assert.False(result);
        }

        [Fact]
        public void SetRainWarningCookie_SetsCookieCorrectly()
        {
            var cityName = "London";
            var context = new DefaultHttpContext();
            _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(context);

            _service.SetRainWarningCookie(cityName);

            var cookieKey = $"RainWarning_{cityName}";

            Assert.True(context.Response.Headers["Set-Cookie"].ToString().Contains(cookieKey), "Cookie was not set correctly.");
        }




    }
}
