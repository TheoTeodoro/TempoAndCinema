using System.Text.Json.Serialization;

namespace TempoAndCinema.Dtos
{
    public class TmdbImageResponseDto
    {
        [JsonPropertyName("backdrops")]
        public List<ImageDto> Backdrops { get; set; } = new();

        [JsonPropertyName("posters")]
        public List<ImageDto> Posters { get; set; } = new();
    }

    public class ImageDto
    {
        [JsonPropertyName("file_path")]
        public string File_Path { get; set; }
    }
}