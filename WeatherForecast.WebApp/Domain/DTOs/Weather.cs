using Newtonsoft.Json;

namespace WeatherForecast.WebApp.Domain.DTOs
{
    public class Weather
    {

        [JsonProperty("main")]
        public string Main { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }
    }
}
