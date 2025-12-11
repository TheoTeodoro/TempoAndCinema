using System.Text.Json.Serialization;

namespace TempoAndCinema.Dtos
{
    public class TmdbMovieDetailsDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("original_title")]
        public string Original_Title { get; set; }

        [JsonPropertyName("overview")]
        public string Overview { get; set; }

        [JsonPropertyName("release_date")]
        public string Release_Date { get; set; } // ou DateTime?

        [JsonPropertyName("runtime")]
        public int? Runtime { get; set; }

        [JsonPropertyName("vote_average")]
        public double Vote_Average { get; set; }

        [JsonPropertyName("poster_path")]
        public string Poster_Path { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string Backdrop_Path { get; set; }

        [JsonPropertyName("original_language")]
        public string Original_Language { get; set; }

        [JsonPropertyName("tagline")]
        public string Tagline { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("genres")]
        public List<GenreDto> Genres { get; set; } = new();

        [JsonPropertyName("production_countries")]
        public List<CountryDto> Production_Countries { get; set; } = new();

        [JsonPropertyName("budget")]
        public long Budget { get; set; }

        [JsonPropertyName("revenue")]
        public long Revenue { get; set; }
    }

    public class GenreDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class CountryDto
    {
        [JsonPropertyName("iso_3166_1")]
        public string IsoCode { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}