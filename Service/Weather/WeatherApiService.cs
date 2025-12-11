using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TempoAndCinema.Dtos;

namespace TempoAndCinema.Service.Weather
{
    public class WeatherApiService : IWeatherApiService
    {
        private readonly HttpClient _httpClient;

        public WeatherApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WeatherDto?> GetWeatherAsync(double latitude, double longitude)
        {
            string url =
       $"/v1/forecast?latitude={latitude.ToString(CultureInfo.InvariantCulture)}" +
       $"&longitude={longitude.ToString(CultureInfo.InvariantCulture)}" +
       "&daily=temperature_2m_max,temperature_2m_min&timezone=auto";


            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<WeatherDto>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }
    }
}
