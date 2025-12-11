using System.Text.Json.Serialization;

public class TmdbSimilarMoviesDto
{
    [JsonPropertyName("results")]
    public List<TmdbSimilarMovieDto> Results { get; set; } = new();
}

public class TmdbSimilarMovieDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("poster_path")]
    public string PosterPath { get; set; }
}