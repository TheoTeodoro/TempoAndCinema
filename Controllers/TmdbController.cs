using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TempoAndCinema.Dtos;
using TempoAndCinema.Services.Tmdb;
using TempoAndCinema.Data;
using TempoAndCinema.Models;

namespace TempoAndCinema.Controllers
{
    public class TmdbController : Controller
    {
        private readonly ITmdbApiService _tmdb;
        private readonly IFilmeRepository _repo;

        public TmdbController(ITmdbApiService tmdb, IFilmeRepository repo)
        {
            _tmdb = tmdb;
            _repo = repo;
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
            var credits = await _tmdb.GetMovieCreditsAsync(tmdbId);
            var videos = await _tmdb.GetMovieVideosAsync(tmdbId);
            var images = await _tmdb.GetMovieImagesAsync(tmdbId);
            var config = await _tmdb.GetConfigurationAsync();

            if (details == null)
                return NotFound("Filme não encontrado no TMDb.");

            var existing = await _repo.GetByTmdbIdAsync(tmdbId);
            if (existing != null)
                return RedirectToAction("Details", "Filmes", new { id = existing.Id });

            var baseUrl = config.Images.Secure_Base_Url;
            var posterSize = config.Images.Poster_Sizes.LastOrDefault() ?? "w500";
            var backdropSize = config.Images.Backdrop_Sizes.LastOrDefault() ?? "w780";

            // --- Trailer ---
            var trailer = videos?.Results?
                .FirstOrDefault(v => v.Type == "Trailer" && v.Site == "YouTube")?
                .Key;

            string trailerUrl = trailer != null
                ? $"https://www.youtube.com/embed/{trailer}"
                : null;

            // --- Backdrops (pega 5 principais) ---
            var backdropUrls = images?.Backdrops?
                .Take(5)
                .Select(b => $"{baseUrl}{backdropSize}{b.File_Path}")
                .ToList();

            // --- Elenco completo como JSON reduzido --- 
            var castList = credits.Cast
                .Select(c => $"{c.Name} ({c.Character})")
                .ToList();

            // Montar objeto
            var filme = new Filme
            {
                TmdbId = details.Id,
                Titulo = details.Title,
                TituloOriginal = details.Original_Title,
                Sinopse = details.Overview,
                DataLancamento = DateTime.TryParse(details.Release_Date, out var date) ? date : null,

                Genero = string.Join(", ", details.Genres.Select(g => g.Name)),
                PosterPath = $"{baseUrl}{posterSize}{details.Poster_Path}",
                Lingua = details.Original_Language,
                Duracao = details.Runtime ?? 0,
                NotaMedia = details.Vote_Average,

                ElencoPrincipal = JsonSerializer.Serialize(castList.Take(5).ToList()),
                TrailerUrl = trailerUrl,
                BackdropsJson = System.Text.Json.JsonSerializer.Serialize(backdropUrls),

                CidadeReferencia = null,
                Latitude = null,
                Longitude = null,

                DataCriacao = DateTime.Now,
                DataAtualizacao = DateTime.Now
            };
            int id = await _repo.AddAsync(filme);
            return RedirectToAction("Details", "Filmes", new { id });
        }
    }
}
