using System.Text.Json.Serialization;

namespace TempoAndCinema.Dtos
{
    public class TmdbCreditsDto
    {
        [JsonPropertyName("cast")]
        public List<CastDto> Cast { get; set; }
        // List<CrewDto> Crew { get; set; }
    }


    public class CastDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("character")]
        public string Character { get; set; }

        [JsonPropertyName("profile_path")]
        public string Profile_Path { get; set; }
    }
}