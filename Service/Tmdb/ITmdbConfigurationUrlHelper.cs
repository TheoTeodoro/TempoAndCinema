using System.Threading.Tasks;

namespace TempoAndCinema.Services.Tmdb
{
    public interface ITmdbConfigurationUrlHelper
    {
        Task<string?> GetPosterUrlAsync(string? posterPath, string size = "w500");
        Task<string?> GetBackdropUrlAsync(string? backdropPath, string size = "w780");
        Task<string?> GetLogoUrlAsync(string? logoPath, string size = "w300");
    }
}