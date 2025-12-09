namespace TempoAndCinema.Dtos
{
    public class FilmeExpandidoDto
    {
        public int TmdbId { get; set; }

        public string Titulo { get; set; }
        public string TituloOriginal { get; set; }
        public string Sinopse { get; set; }
        public string DataLancamento { get; set; }
        public string GeneroPrincipal { get; set; }

        public string PosterPath { get; set; }
        public string Lingua { get; set; }

        public int? Duracao { get; set; }
        public double NotaMedia { get; set; }

        public string TrailerUrl { get; set; }              
        public List<string> Backdrops { get; set; } = new(); 
        public List<string> Elenco { get; set; } = new();     
    }
}