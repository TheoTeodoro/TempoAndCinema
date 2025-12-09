using System.Text.Json.Serialization;

namespace TempoAndCinema.Dtos
{
    public class TmdbVideosDto
    {
        [JsonPropertyName("results")]
        public List<VideoDto> Results { get; set; } = new();
    }

    public class VideoDto
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("site")]
        public string Site { get; set; }
    }
}