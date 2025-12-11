using TempoAndCinema.Dtos;
using TempoAndCinema.Models;

namespace TempoAndCinema.ViewModels
{
    public class HomePageViewModel
    {
        public IEnumerable<FilmeExpandidoDto> Trending { get; set; }
        public IEnumerable<FilmeExpandidoDto> Lancamentos { get; set; }

        // Catálogo local (Filmes já importados)
        public IEnumerable<Filme> Catalogo { get; set; }
    }
}