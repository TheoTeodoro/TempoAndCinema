using System.Threading.Tasks;
using System.Collections.Generic;
using TempoAndCinema.Dtos;

namespace TempoAndCinema.Services.Tmdb
{
    public interface ITmdbApiService
    {
        Task<TmdbCreditsDto?> GetMovieCreditsAsync(int movieId);
        Task<TmdbVideosDto?> GetMovieVideosAsync(int movieId);
        Task<TmdbSearchResponseDto?> SearchMoviesAsync(string query, int page = 1);
        Task<TmdbMovieDetailsDto?> GetMovieDetailsAsync(int movieId);
        Task<TmdbImageResponseDto?> GetMovieImagesAsync(int movieId);
        Task<TmdbConfigurationDto?> GetConfigurationAsync();
        Task<IEnumerable<FilmeExpandidoDto>> GetTrendingMoviesAsync();
        Task<IEnumerable<FilmeExpandidoDto>> GetNowPlayingMoviesAsync();
    }
}