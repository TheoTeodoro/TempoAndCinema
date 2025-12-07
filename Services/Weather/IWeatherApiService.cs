using System.Threading.Tasks;
using TempoAndCinema.Dtos;

namespace TempoAndCinema.Services.Weather
{
    public interface IWeatherApiService
    {
        Task<WeatherDto?> GetWeatherAsync(double latitude, double longitude);
    }
}
