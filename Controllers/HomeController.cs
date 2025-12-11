using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TempoAndCinema.Models;
<<<<<<< HEAD
using TempoAndCinema.Services.Tmdb;
using TempoAndCinema.Data;
using TempoAndCinema.Dtos;
using TempoAndCinema.ViewModels;
=======
>>>>>>> origin/LucayanBranch

namespace TempoAndCinema.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
<<<<<<< HEAD
        private readonly ITmdbApiService _tmdb;
        private readonly IFilmeRepository _filmeRepository;

        public HomeController(ILogger<HomeController> logger, ITmdbApiService tmdb, IFilmeRepository filmeRepository)
        {
            _logger = logger;
            _tmdb = tmdb;
            _filmeRepository = filmeRepository;
        }

        public async Task<IActionResult> Index()
        {
            var trending = await _tmdb.GetTrendingMoviesAsync();
            var lancamentos = await _tmdb.GetNowPlayingMoviesAsync();
            var catalogo = await _filmeRepository.GetAllAsync();

            var vm = new HomePageViewModel
            {
                Trending = trending.ToList(),
                Lancamentos = lancamentos.ToList(),
                Catalogo = catalogo
            };

            return View(vm);
        }


        public IActionResult Privacy() => View();
=======

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
>>>>>>> origin/LucayanBranch

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
