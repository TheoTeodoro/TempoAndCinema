using System.Text.Json.Serialization;

namespace TempoAndCinema.Dtos
{
    public class TmdbTrendingResponseDto
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("results")]
        public List<TmdbMovieResultDto> Results { get; set; } = new();

        [JsonPropertyName("total_pages")]
        public int Total_Pages { get; set; }

        [JsonPropertyName("total_results")]
        public int Total_Results { get; set; }
    }
}