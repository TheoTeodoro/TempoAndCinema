using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TempoAndCinema.Dtos;
using TempoAndCinema.Services.Tmdb;
using TempoAndCinema.Data;
using TempoAndCinema.Models;
using TempoAndCinema.Service.Weather;

namespace TempoAndCinema.Controllers
{
    public class TmdbController : Controller
    {
        private readonly ITmdbApiService _tmdb;
        private readonly IFilmeRepository _repo;

        private readonly IWeatherApiService _weatherService;


        public TmdbController(ITmdbApiService tmdb, IFilmeRepository repo, IWeatherApiService weather)
        {
            _tmdb = tmdb;
            _repo = repo;
            _weatherService = weather;
        }


        // RF02 — Tela inicial de busca
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: /Tmdb/Search
        [HttpGet]
        public async Task<IActionResult> Search(string query, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(query))
                return View(new TmdbSearchResponseDto()); // tela vazia

            var response = await _tmdb.SearchMoviesAsync(query, page);
            response.Query = query;

            return View(response);
        }

        // RF04 — Detalhes do filme via TMDb
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _tmdb.GetMovieDetailsAsync(id);
            if (movie == null) return NotFound();
            var existing = await _repo.GetByTmdbIdAsync(id);

            if (existing != null && existing.Latitude.HasValue && existing.Longitude.HasValue)
            {
                var clima = await _weatherService.GetWeatherAsync(
                    existing.Latitude.Value,
                    existing.Longitude.Value
                );
                ViewBag.Weather = clima;
            }
            else
            {
                ViewBag.Weather = null;
            }
            var images = await _tmdb.GetMovieImagesAsync(id);
            var credits = await _tmdb.GetMovieCreditsAsync(id);
            var videos = await _tmdb.GetMovieVideosAsync(id);
            var config = await _tmdb.GetConfigurationAsync();

            var baseUrl = config?.Images?.Secure_Base_Url ?? "https://image.tmdb.org/t/p/";
            var posterSize = config?.Images?.Poster_Sizes?.LastOrDefault() ?? "w500";
            var backdropSize = config?.Images?.Backdrop_Sizes?.LastOrDefault() ?? "w780";

            ViewBag.BaseUrl = baseUrl;
            ViewBag.PosterSize = posterSize;
            ViewBag.BackdropSize = backdropSize;
            ViewBag.Images = images;
            ViewBag.Credits = credits;
            ViewBag.Videos = videos;

            return View(movie);
        }



        [HttpPost]
        public async Task<IActionResult> ImportToLocal(int tmdbId)
        {
            var details = await _tmdb.GetMovieDetailsAsync(tmdbId);
            if (details == null)
                return NotFound("Filme não encontrado no TMDb.");

            var existing = await _repo.GetByTmdbIdAsync(tmdbId);
            if (existing != null)
                return RedirectToAction("Details", "Filmes", new { id = existing.Id });

            // REDIRECIONA para tela de preenchimento de geolocalização
            return RedirectToAction(
                "AdicionarLocalizacao",
                new
                {
                    tmdbId = tmdbId,
                    titulo = details.Title,
                    poster = details.Poster_Path
                }
            );
        }

        public IActionResult AdicionarLocalizacao(int tmdbId, string titulo, string poster)
        {
            ViewBag.TmdbId = tmdbId;
            ViewBag.Titulo = titulo;
            ViewBag.Poster = poster;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SalvarComLocalizacao(int tmdbId,string cidade,string latitude,string longitude)
        {
            // Parse manual usando ponto como decimal
            if (!double.TryParse(latitude, NumberStyles.Any, CultureInfo.InvariantCulture, out var lat))
                return BadRequest("Latitude inválida. Use ponto como separador decimal.");

            if (!double.TryParse(longitude, NumberStyles.Any, CultureInfo.InvariantCulture, out var lng))
                return BadRequest("Longitude inválida. Use ponto como separador decimal.");

            var details = await _tmdb.GetMovieDetailsAsync(tmdbId);
            var credits = await _tmdb.GetMovieCreditsAsync(tmdbId);
            var videos = await _tmdb.GetMovieVideosAsync(tmdbId);
            var images = await _tmdb.GetMovieImagesAsync(tmdbId);
            var config = await _tmdb.GetConfigurationAsync();

            var filme = new Filme
            {
                TmdbId = details.Id,
                Titulo = details.Title,
                TituloOriginal = details.Original_Title,
                Sinopse = details.Overview,
                DataLancamento = DateTime.TryParse(details.Release_Date, out var date) ? date : null,

                Genero = string.Join(", ", details.Genres.Select(g => g.Name)),
                PosterPath = $"{config.Images.Secure_Base_Url}{config.Images.Poster_Sizes.Last()}{details.Poster_Path}",
                Lingua = details.Original_Language,
                Duracao = details.Runtime ?? 0,
                NotaMedia = details.Vote_Average,

                CidadeReferencia = cidade,
                Latitude = lat,
                Longitude = lng,

                DataCriacao = DateTime.Now,
                DataAtualizacao = DateTime.Now
            };

            int id = await _repo.AddAsync(filme);
            return RedirectToAction("Details", "Filmes", new { id });
        }

    }
}
