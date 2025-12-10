using System.Text.Json.Serialization;

namespace TempoAndCinema.Dtos
{
    public class TmdbNowPlayingResponseDto
    {
        [JsonPropertyName("dates")]
        public NowPlayingDatesDto Dates { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("results")]
        public List<TmdbMovieResultDto> Results { get; set; } = new();

        [JsonPropertyName("total_pages")]
        public int Total_Pages { get; set; }

        [JsonPropertyName("total_results")]
        public int Total_Results { get; set; }
    }

    public class NowPlayingDatesDto
    {
        [JsonPropertyName("maximum")]
        public string Maximum { get; set; }

        [JsonPropertyName("minimum")]
        public string Minimum { get; set; }
    }
}