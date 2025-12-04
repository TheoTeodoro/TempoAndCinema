using Microsoft.AspNetCore.Mvc;
using TempoAndCinema.Services.Tmdb;

namespace TempoAndCinema.Controllers
{
    public class TmdbController : Controller
    {
        private readonly ITmdbApiService _tmdb;

        public TmdbController(ITmdbApiService tmdb)
        {
            _tmdb = tmdb;
        }
        
        public async Task<IActionResult> Index()
        {
            return View();
        }
        
        // RF02 — Busca de filmes
        public async Task<IActionResult> Search(string query, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(query))
                return View(null);

            var result = await _tmdb.SearchMoviesAsync(query, page);

            return View(result);
        }
        
        //RF04 — Detalhes do filme
        public async Task<IActionResult> Details(int id)
        {
            var details = await _tmdb.GetMovieDetailsAsync(id);
            var images  = await _tmdb.GetMovieImagesAsync(id);

            ViewBag.Images = images; // opcional, para galeria
            return View(details);
        }
    }
}