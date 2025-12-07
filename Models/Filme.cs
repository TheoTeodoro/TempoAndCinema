using System.ComponentModel.DataAnnotations;
namespace TempoAndCinema.Models;

public class Filme
{
    public int Id { get; set; }
    public int? TmdbId { get; set; }

    [Required(ErrorMessage = "Título é obrigatório")]
    public string Titulo { get; set; }

    [Required(ErrorMessage = "Título original é obrigatório")]
    public string TituloOriginal { get; set; }

    [Required(ErrorMessage = "Sinopse é obrigatória")]
    public string Sinopse { get; set; }

    public DateTime? DataLancamento { get; set; }

    [Required(ErrorMessage = "Gênero é obrigatório")]
    public string Genero { get; set; }

    [Required(ErrorMessage = "Poster é obrigatório")]
    public string PosterPath { get; set; }

    [Required(ErrorMessage = "Idioma é obrigatório")]
    public string Lingua { get; set; }

    public int? Duracao { get; set; }
    public double? NotaMedia { get; set; }

    public string? ElencoPrincipal { get; set; }
    public string? CidadeReferencia { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    
    public string? TrailerUrl { get; set; }
    public string? CastUrl { get; set; }
    public string? BackdropsJson { get; set; }

    public DateTime DataCriacao { get; set; }
    public DateTime DataAtualizacao { get; set; }
}