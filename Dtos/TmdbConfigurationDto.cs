using System.Text.Json.Serialization;

namespace TempoAndCinema.Dtos
{
    public class TmdbConfigurationDto
    {
        [JsonPropertyName("images")]
        public TmdbImageConfigDto Images { get; set; } = new();
    }

    public class TmdbImageConfigDto
    {
        [JsonPropertyName("base_url")]
        public string Base_Url { get; set; }

        [JsonPropertyName("secure_base_url")]
        public string Secure_Base_Url { get; set; }

        [JsonPropertyName("backdrop_sizes")]
        public List<string> Backdrop_Sizes { get; set; } = new();

        [JsonPropertyName("logo_sizes")]
        public List<string> Logo_Sizes { get; set; } = new();

        [JsonPropertyName("poster_sizes")]
        public List<string> Poster_Sizes { get; set; } = new();

        [JsonPropertyName("profile_sizes")]
        public List<string> Profile_Sizes { get; set; } = new();

        [JsonPropertyName("still_sizes")]
        public List<string> Still_Sizes { get; set; } = new();
    }
}