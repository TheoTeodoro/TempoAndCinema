using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using TempoAndCinema.Models;
using TempoAndCinema.Dtos;
using TempoAndCinema.Service;

namespace TempoAndCinema.Services.Tmdb
{
    public class TmdbApiService : ITmdbApiService
    {
        private readonly HttpClient _http;
        private readonly IMemoryCache _cache;
        private readonly string _apiKey;
        private readonly JsonSerializerOptions _jsonOptions;

        private const string BaseUrl = "https://api.themoviedb.org/3";

        public TmdbApiService(HttpClient http, IMemoryCache cache, IConfiguration config)
        {
            _http = http;
            _cache = cache;
            _apiKey = config["TMDB:ApiKey"] ?? throw new Exception("TMDB ApiKey missing!");

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        // ============================================================
        // RF02 /search/movie  (cache 5 min)
        // ============================================================
        public async Task<TmdbSearchResponseDto?> SearchMoviesAsync(string query, int page = 1)
        {
            string cacheKey = $"tmdb_search_{query}_{page}";

            if (_cache.TryGetValue(cacheKey, out TmdbSearchResponseDto cached))
                return cached;

            string url = $"{BaseUrl}/search/movie?api_key={_apiKey}&language=pt-BR&query={query}&page={page}";

            var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<TmdbSearchResponseDto>(json, _jsonOptions);

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5)); // RF08

            return result;
        }

        // ============================================================
        // RF04 /movie/{id}  (cache 10 min)
        // ============================================================
        public async Task<TmdbMovieDetailsDto?> GetMovieDetailsAsync(int movieId)
        {
            string cacheKey = $"tmdb_details_{movieId}";

            if (_cache.TryGetValue(cacheKey, out TmdbMovieDetailsDto cached))
                return cached;

            string url = $"{BaseUrl}/movie/{movieId}?api_key={_apiKey}&language=pt-BR";

            var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<TmdbMovieDetailsDto>(json, _jsonOptions);

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }

        // ============================================================
        // RF04 /movie/{id}/images  (cache 10 min)
        // ============================================================
        public async Task<TmdbImageResponseDto?> GetMovieImagesAsync(int movieId)
        {
            string cacheKey = $"tmdb_images_{movieId}";

            if (_cache.TryGetValue(cacheKey, out TmdbImageResponseDto cached))
                return cached;

            string url = $"{BaseUrl}/movie/{movieId}/images?api_key={_apiKey}";

            var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<TmdbImageResponseDto>(json, _jsonOptions);

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }

        // ============================================================
        // RF08 /configuration (cache obrigatório)
        // Usado para montar URL final do poster
        // ============================================================
        public async Task<TmdbConfigurationDto?> GetConfigurationAsync()
        {
            const string cacheKey = "tmdb_configuration";

            if (_cache.TryGetValue(cacheKey, out TmdbConfigurationDto cached))
                return cached;

            string url = $"{BaseUrl}/configuration?api_key={_apiKey}";

            var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<TmdbConfigurationDto>(json, _jsonOptions);

            // Pode ficar 12h no cache (não muda frequentemente).
            _cache.Set(cacheKey, result, TimeSpan.FromHours(12));

            return result;
        }
        
        public async Task<TmdbCreditsDto?> GetMovieCreditsAsync(int movieId)
        {
            string cacheKey = $"tmdb_credits_{movieId}";

            if (_cache.TryGetValue(cacheKey, out TmdbCreditsDto cached))
                return cached;

            string url = $"{BaseUrl}/movie/{movieId}/credits?api_key={_apiKey}&language=pt-BR";

            var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<TmdbCreditsDto>(json, _jsonOptions);

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(30));

            return result;
        }
        
        public async Task<TmdbVideosDto?> GetMovieVideosAsync(int movieId)
        {
            string cacheKey = $"tmdb_videos_{movieId}";

            if (_cache.TryGetValue(cacheKey, out TmdbVideosDto cached))
                return cached;

            string url = $"{BaseUrl}/movie/{movieId}/videos?api_key={_apiKey}&language=pt-BR";

            var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<TmdbVideosDto>(json, _jsonOptions);

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(30));

            return result;
        }
        
        public async Task<TmdbReviewResponseDto?> GetMovieReviewsAsync(int movieId)
        {
            string url = $"{BaseUrl}/movie/{movieId}/reviews?api_key={_apiKey}&language=pt-BR";

            var response = await _http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;  // evita quebrar a aplicação

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TmdbReviewResponseDto>(json);
        }

        public async Task<TmdbSimilarMoviesDto?> GetSimilarMoviesAsync(int movieId)
        {
            string url = $"{BaseUrl}/movie/{movieId}/similar?api_key={_apiKey}&language=pt-BR";
            
                var response = await _http.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TmdbSimilarMoviesDto>(json);

            return result;
        }

        
        // === Trending ===
        public async Task<IEnumerable<FilmeExpandidoDto>> GetTrendingMoviesAsync()
        {
            string cacheKey = "tmdb_trending";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<FilmeExpandidoDto> cached))
                return cached;

            string url = $"{BaseUrl}/trending/movie/week?api_key={_apiKey}&language=pt-BR";

            var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<TmdbTrendingResponseDto>(json, _jsonOptions);

            if (result?.Results == null)
                return Enumerable.Empty<FilmeExpandidoDto>();

            var filmes = result.Results.Select(m => new FilmeExpandidoDto
            {
                TmdbId = m.Id,
                Titulo = m.Title ?? "",
                NotaMedia = m.Vote_Average,
                PosterPath = m.Poster_Path
            }).ToList();

            _cache.Set(cacheKey, filmes, TimeSpan.FromMinutes(10));

            return filmes;
        }

        
        // === Now Playing ===
        public async Task<IEnumerable<FilmeExpandidoDto>> GetNowPlayingMoviesAsync()
        {
            string cacheKey = "tmdb_nowplaying";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<FilmeExpandidoDto> cached))
                return cached;

            string url = $"{BaseUrl}/movie/now_playing?api_key={_apiKey}&language=pt-BR&page=1";

            var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<TmdbNowPlayingResponseDto>(json, _jsonOptions);

            if (result?.Results == null)
                return Enumerable.Empty<FilmeExpandidoDto>();

            var filmes = result.Results.Select(m => new FilmeExpandidoDto
            {
                TmdbId = m.Id,
                Titulo = m.Title ?? "",
                NotaMedia = m.Vote_Average,
                PosterPath = m.Poster_Path
            }).ToList();

            _cache.Set(cacheKey, filmes, TimeSpan.FromMinutes(10));

            return filmes;
        }

    }
    
    // === DTO usado somente pelo Trending e NowPlaying ===
    public class TmdbTrendingResponseDto
    {
        public List<TmdbTrendingMovieDto>? Results { get; set; }
    }

    public class TmdbTrendingMovieDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Poster_Path { get; set; }
        public double Vote_Average { get; set; }
    }

}
