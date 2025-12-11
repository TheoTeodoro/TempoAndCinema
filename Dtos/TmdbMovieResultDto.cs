using System.Text.Json.Serialization;

namespace TempoAndCinema.Dtos
{
    public class TmdbMovieResultDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("original_title")]
        public string OriginalTitle { get; set; }

        [JsonPropertyName("overview")]
        public string Overview { get; set; }

        [JsonPropertyName("poster_path")]
        public string Poster_Path { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string BackdropPath { get; set; }

        [JsonPropertyName("vote_average")]
        public double Vote_Average { get; set; }

        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; }
    }
}