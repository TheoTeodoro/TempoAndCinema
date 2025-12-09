using System.ComponentModel.DataAnnotations;

namespace TempoAndCinema.Models
{
    public class Filme
    {
        public int Id { get; set; }

        // --- IDENTIFICAÇÃO ---
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


        // --- CAMPOS QUE VOCÊ JÁ TINHA ---
        public string? ElencoPrincipal { get; set; }        // lista rápida
        public string? CidadeReferencia { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string? TrailerUrl { get; set; }
        public string? CastUrl { get; set; }

        // backdrops, imagens etc do TMDb
        public string? BackdropsJson { get; set; }          // armazenar JSON direto


        // --- NOVOS CAMPOS NECESSÁRIOS PARA "SALVAR TUDO" DO TMDB ---
        public string? GenerosJson { get; set; }            // lista de gêneros do DTO
        public string? ProducaoJson { get; set; }           // production_companies / production_countries
        public string? SpokenLanguagesJson { get; set; }     // idiomas falados
        public string? KeywordsJson { get; set; }            // palavras-chave
        public string? VideosJson { get; set; }              // trailers e outros vídeos
        public string? CreditsJson { get; set; }             // todo o elenco e equipe
        public string? ImagesJson { get; set; }              // posters, backdrops, logos
        public string? RecommendationsJson { get; set; }     // filmes recomendados
        public string? SimilarJson { get; set; }             // filmes similares


        // --- METADADOS ---
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}
