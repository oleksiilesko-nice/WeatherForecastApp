using Newtonsoft.Json;

namespace WeatherForecast.WebApp.Domain.DTOs
{
    public class Wind
    {
        [JsonProperty("speed")]
        public double? Speed { get; set; }

        [JsonProperty("deg")]
        public int? Deg { get; set; }
    }
}
