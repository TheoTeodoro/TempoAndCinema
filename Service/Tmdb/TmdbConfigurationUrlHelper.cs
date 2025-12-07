using TempoAndCinema.Dtos;

namespace TempoAndCinema.Services.Tmdb
{
    public class TmdbConfigurationUrlHelper : ITmdbConfigurationUrlHelper
    {
        private readonly ITmdbApiService _tmdbApi;

        public TmdbConfigurationUrlHelper(ITmdbApiService tmdbApi)
        {
            _tmdbApi = tmdbApi;
        }

        public async Task<string?> GetPosterUrlAsync(string? posterPath, string size = "w500")
        {
            if (string.IsNullOrWhiteSpace(posterPath))
                return null;

            var config = await _tmdbApi.GetConfigurationAsync();
            if (config == null)
                return null;

            string baseUrl = config.Images.Secure_Base_Url;
            if (!config.Images.Poster_Sizes.Contains(size))
                size = config.Images.Poster_Sizes.FirstOrDefault("w500");

            return $"{baseUrl}{size}{posterPath}";
        }

        public async Task<string?> GetBackdropUrlAsync(string? backdropPath, string size = "w780")
        {
            if (string.IsNullOrWhiteSpace(backdropPath))
                return null;

            var config = await _tmdbApi.GetConfigurationAsync();
            if (config == null)
                return null;

            string baseUrl = config.Images.Secure_Base_Url;
            if (!config.Images.Backdrop_Sizes.Contains(size))
                size = config.Images.Backdrop_Sizes.FirstOrDefault("w780");

            return $"{baseUrl}{size}{backdropPath}";
        }

        public async Task<string?> GetLogoUrlAsync(string? logoPath, string size = "w300")
        {
            if (string.IsNullOrWhiteSpace(logoPath))
                return null;

            var config = await _tmdbApi.GetConfigurationAsync();
            if (config == null)
                return null;

            string baseUrl = config.Images.Secure_Base_Url;
            if (!config.Images.Logo_Sizes.Contains(size))
                size = config.Images.Logo_Sizes.FirstOrDefault("w300");

            return $"{baseUrl}{size}{logoPath}";
        }
    }
}
