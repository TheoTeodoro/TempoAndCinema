using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TempoAndCinema.Dtos;
using TempoAndCinema.Services.Tmdb;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<TmdbController> _logger;


        public TmdbController(ITmdbApiService tmdb, IFilmeRepository repo, IWeatherApiService weather, ILogger<TmdbController> logger)
        {
            _tmdb = tmdb;
            _repo = repo;
            _weatherService = weather;
            _logger = logger;
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

            // verifica se já existe no repo para buscar clima
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

            // chamadas TMDb adicionais
            var images = await _tmdb.GetMovieImagesAsync(id);
            var credits = await _tmdb.GetMovieCreditsAsync(id);
            var videos = await _tmdb.GetMovieVideosAsync(id);
            var similar = await _tmdb.GetSimilarMoviesAsync(id);      // <-- filmes semelhantes
            var reviews = await _tmdb.GetMovieReviewsAsync(id);      // <-- reviews / avaliações
            var config = await _tmdb.GetConfigurationAsync();

            var baseUrl = config?.Images?.Secure_Base_Url ?? "https://image.tmdb.org/t/p/";

            // Choose the best available sizes from TMDb configuration (prefer numeric widths)
            string ChooseClosestSize(IEnumerable<string>? sizes, int desired)
            {
                if (sizes == null) return desired == 780 ? "w780" : "w500";

                var numericSizes = sizes
                    .Where(s => s != null && s.StartsWith("w") && int.TryParse(s.Substring(1), out _))
                    .Select(s => int.Parse(s.Substring(1)))
                    .ToList();

                if (!numericSizes.Any())
                {
                    // fallback to a reasonable default
                    return desired == 780 ? "w780" : "w500";
                }

                // pick the size with the minimal absolute difference to desired
                var best = numericSizes.OrderBy(n => Math.Abs(n - desired)).First();
                return "w" + best;
            }

            // Prefer sizes near these targets, but pick whatever TMDb provides closest match
            var posterSize = ChooseClosestSize(config?.Images?.Poster_Sizes, 500);
            var backdropSize = ChooseClosestSize(config?.Images?.Backdrop_Sizes, 780);

            // enviar para a view via ViewBag
            ViewBag.BaseUrl = baseUrl;
            ViewBag.PosterSize = posterSize;
            ViewBag.BackdropSize = backdropSize;
            ViewBag.Images = images;
            ViewBag.Credits = credits;
            ViewBag.Videos = videos;
            ViewBag.Similar = similar;
            ViewBag.Reviews = reviews;

            // Log chosen sizes and example constructed image URLs to help debugging
            try
            {
                var postersCount = images?.Posters?.Count ?? 0;
                var backdropsCount = images?.Backdrops?.Count ?? 0;
                var similarCount = similar?.Results?.Count ?? 0;

                var examplePosterPath = images?.Posters?.FirstOrDefault()?.File_Path;
                var exampleBackdropPath = images?.Backdrops?.FirstOrDefault()?.File_Path;
                var exampleSimilarPosterPath = similar?.Results?.FirstOrDefault()?.PosterPath;

                var examplePosterUrl = examplePosterPath != null ? $"{baseUrl}{posterSize}{examplePosterPath}" : "n/a";
                var exampleBackdropUrl = exampleBackdropPath != null ? $"{baseUrl}{backdropSize}{exampleBackdropPath}" : "n/a";
                var exampleSimilarPosterUrl = exampleSimilarPosterPath != null ? $"{baseUrl}{posterSize}{exampleSimilarPosterPath}" : "n/a";

                _logger.LogInformation("TMDb sizes chosen: Poster={PosterSize}, Backdrop={BackdropSize}. Counts: Posters={PostersCount}, Backdrops={BackdropsCount}, Similar={SimilarCount}. Example URLs: Poster={PosterUrl}, Backdrop={BackdropUrl}, SimilarPoster={SimilarPosterUrl}",
                    posterSize, backdropSize, postersCount, backdropsCount, similarCount, examplePosterUrl, exampleBackdropUrl, exampleSimilarPosterUrl);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to log TMDb image debug info");
            }

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

                // 🔥 SALVANDO ELENCO
                ElencoPrincipal = JsonSerializer.Serialize(
                    credits?.Cast?.Select(c => c.Name).Take(10).ToList()
                ),

                // 🔥 SALVANDO BACKDROPS
                BackdropsJson = JsonSerializer.Serialize(
                    images?.Backdrops?.Select(b =>
                        $"{config.Images.Secure_Base_Url}{config.Images.Backdrop_Sizes.Last()}{b.File_Path}"
                    )
                ),

                // 🔥 SALVANDO POSTERS (usado depois)
                ImagesJson = JsonSerializer.Serialize(
                    images?.Posters?.Select(p =>
                        $"{config.Images.Secure_Base_Url}{config.Images.Poster_Sizes.Last()}{p.File_Path}"
                    )
                ),

                // 🔥 SALVANDO TUDO (opcional)
                CreditsJson = JsonSerializer.Serialize(credits),
                VideosJson = JsonSerializer.Serialize(videos),

                DataCriacao = DateTime.Now,
                DataAtualizacao = DateTime.Now
            };


            int id = await _repo.AddAsync(filme);
            return RedirectToAction("Details", "Filmes", new { id });
        }

    }
}
