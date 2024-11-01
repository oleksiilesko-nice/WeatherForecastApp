using Newtonsoft.Json;

namespace WeatherForecast.WebApp.Domain.DTOs
{
    public class Coord
    {
        [JsonProperty("lon")]
        public double? Lon { get; set; }

        [JsonProperty("lat")]
        public double? Lat { get; set; }
    }
}
