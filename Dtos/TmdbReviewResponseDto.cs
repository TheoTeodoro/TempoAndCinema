using System.Text.Json.Serialization;

public class TmdbReviewResponseDto
{
    [JsonPropertyName("results")]
    public List<TmdbReviewDto> Results { get; set; } = new();
}

public class TmdbReviewDto
{
    [JsonPropertyName("author")]
    public string Author { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; }
}