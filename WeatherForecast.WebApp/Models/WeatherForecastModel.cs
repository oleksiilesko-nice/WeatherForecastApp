using System.ComponentModel.DataAnnotations;

namespace WeatherForecast.WebApp.Models
{
    public class WeatherForecastModel
    {
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }

        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public double MinTemperature { get; set; }
        public double MaxTemperature { get; set; }

        public string Condition { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
        public int SeaLevel { get; set; }
        public int GrndLevel { get; set; }

        public int Visibility { get; set; }
        public double WindSpeed { get; set; }
        public int WindDirection { get; set; }
        public int CloudCoverage { get; set; }

        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }
        public string Country { get; set; }
        public string CityName { get; set; }
        public bool IsRainExpected { get; set; } 
        public string LastUsedCity { get; set; }
    }



}
