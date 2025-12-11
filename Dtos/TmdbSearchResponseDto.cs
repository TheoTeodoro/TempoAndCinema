using System.Text.Json.Serialization;

namespace TempoAndCinema.Dtos
{
    // Representa a resposta completa de /search/movie
    public class TmdbSearchResponseDto
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("total_results")]
        public int TotalResults { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }
        public string Query { get; set; }

        [JsonPropertyName("results")]
        public List<MovieResultDto> Results { get; set; } = new();
    }

    // Representa cada filme da lista
    public class MovieResultDto
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
        public string Release_Date { get; set; }

        [JsonPropertyName("genre_ids")]
        public List<int> Genre_Ids { get; set; } = new();

        [JsonPropertyName("poster_path")]
        public string Poster_Path { get; set; }

        [JsonPropertyName("original_language")]
        public string Original_Language { get; set; }

        [JsonPropertyName("vote_average")]
        public double Vote_Average { get; set; }
    }
}