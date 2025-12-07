using TempoAndCinema.Models;
using TempoAndCinema.Dtos;

namespace TempoAndCinema.Services.Tmdb
{
    public static class TmdbMapper
    {
        public static Filme ToFilme(TmdbMovieDetailsDto dto)
        {
            return new Filme
            {
                TmdbId = dto.Id,
                Titulo = dto.Title,
                TituloOriginal = dto.Original_Title,
                Sinopse = dto.Overview,
                DataLancamento = string.IsNullOrWhiteSpace(dto.Release_Date)
                    ? null
                    : DateTime.Parse(dto.Release_Date),
                Genero = string.Join(", ", dto.Genres.Select(g => g.Name)),
                PosterPath = dto.Poster_Path,
                Lingua = dto.Original_Language,
                Duracao = dto.Runtime,
                NotaMedia = dto.Vote_Average,
                ElencoPrincipal = "", // depois iremos puxar o elenco com /credits
                CidadeReferencia = "",
                Latitude = null,
                Longitude = null,
            };
        }
    }
}