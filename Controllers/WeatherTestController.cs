using Microsoft.AspNetCore.Mvc;
using TempoAndCinema.Services.Weather;

namespace TempoAndCinema.Controllers
{
	public class WeatherTestController : Controller
	{
		private readonly IWeatherApiService _weatherService;

		public WeatherTestController(IWeatherApiService weatherService)
		{
			_weatherService = weatherService;
		}

		// /WeatherTest/Teste?lat=-23.55&lon=-46.63
		public async Task<IActionResult> Test(double lat, double lon)
		{
			var result = await _weatherService.GetWeatherAsync(lat, lon);

			if (result == null || result.Daily == null)
				return View(null);

			// Agora enviamos o próprio WeatherResponse para a View
			return View(result);
		}
	}
}
