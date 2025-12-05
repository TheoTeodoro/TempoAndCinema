using System.Text.Json.Serialization;

namespace TempoAndCinema.Dtos
{

    public class WeatherDto
    {
        public DailyWeather Daily { get; set; }
    }

    public class DailyWeather
    {
        public List<string> Time { get; set; }
        public List<double> Temperature_2m_Max { get; set; }
        public List<double> Temperature_2m_Min { get; set; }
    }
}
