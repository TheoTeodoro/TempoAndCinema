using Dapper;
using Microsoft.Data.Sqlite;
using TempoAndCinema.Models;

namespace TempoAndCinema.Data
{
    public class FilmeRepository : IFilmeRepository
    {
        private readonly string _connectionString;

        public FilmeRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new Exception("Connection string 'DefaultConnection' não encontrada.");
        }

        private SqliteConnection CreateConnection()
            => new SqliteConnection(_connectionString);

        public async Task<List<Filme>> GetAllAsync()
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();
            string sql = @"SELECT * FROM Filmes ORDER BY DataCriacao DESC";
            var result = await conn.QueryAsync<Filme>(sql);
            return result.ToList();
        }

        public async Task<Filme?> GetByIdAsync(int id)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();
            string sql = @"SELECT * FROM Filmes WHERE Id = @Id";
            return await conn.QueryFirstOrDefaultAsync<Filme>(sql, new { Id = id });
        }

        public async Task<Filme?> GetByTmdbIdAsync(int tmdbId)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();
            string sql = @"SELECT * FROM Filmes WHERE TmdbId = @TmdbId";
            return await conn.QueryFirstOrDefaultAsync<Filme>(sql, new { TmdbId = tmdbId });
        }

        public async Task<int> AddAsync(Filme filme)
        {
            using var conn = CreateConnection();
            // force open
            await conn.OpenAsync();

            string sql = @"
                INSERT INTO Filmes 
                (TmdbId, Titulo, TituloOriginal, Sinopse, DataLancamento, Genero, PosterPath,
                Lingua, Duracao, NotaMedia, TrailerUrl, ElencoPrincipal, BackdropsJson,
                CidadeReferencia, Latitude, Longitude, DataCriacao, DataAtualizacao)
                VALUES
                (@TmdbId, @Titulo, @TituloOriginal, @Sinopse, @DataLancamento, @Genero, @PosterPath,
                @Lingua, @Duracao, @NotaMedia, @TrailerUrl, @ElencoPrincipal, @BackdropsJson,
                @CidadeReferencia, @Latitude, @Longitude, @DataCriacao, @DataAtualizacao);

                SELECT last_insert_rowid();
            ";


            var parameters = new
            {
                filme.TmdbId,
                filme.Titulo,
                filme.TituloOriginal,
                filme.Sinopse,
                DataLancamento = filme.DataLancamento?.ToString("yyyy-MM-dd"),
                filme.Genero,
                filme.PosterPath,
                filme.Lingua,
                filme.Duracao,
                filme.NotaMedia,

                filme.ElencoPrincipal,
                BackdropsJson = filme.BackdropsJson,
                filme.TrailerUrl,

                filme.CidadeReferencia,
                filme.Latitude,
                filme.Longitude,

                DataCriacao = filme.DataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                DataAtualizacao = filme.DataAtualizacao.ToString("yyyy-MM-dd HH:mm:ss")
            };
            
            var scalar = await conn.ExecuteScalarAsync<long>(sql, parameters);
            return (int)scalar;
        }

        public async Task UpdateAsync(Filme filme)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();

            string sql = @"
        UPDATE Filmes SET
            Titulo = @Titulo,
            TituloOriginal = @TituloOriginal,
            Sinopse = @Sinopse,
            DataLancamento = @DataLancamento,
            Genero = @Genero,
            PosterPath = @PosterPath,
            Lingua = @Lingua,
            Duracao = @Duracao,
            NotaMedia = @NotaMedia,
            ElencoPrincipal = @ElencoPrincipal,
            CidadeReferencia = @CidadeReferencia,
            Latitude = @Latitude,
            Longitude = @Longitude,
            TrailerUrl = @TrailerUrl,
            DataAtualizacao = @DataAtualizacao
        WHERE Id = @Id
    ";

            var parameters = new
            {
                filme.Id,
                filme.Titulo,
                filme.TituloOriginal,
                filme.Sinopse,
                DataLancamento = filme.DataLancamento?.ToString("yyyy-MM-dd"),
                filme.Genero,
                filme.PosterPath,
                filme.Lingua,
                filme.Duracao,
                filme.NotaMedia,
                filme.ElencoPrincipal,
                filme.CidadeReferencia,
                filme.Latitude,
                filme.Longitude,
                filme.TrailerUrl,
                DataAtualizacao = filme.DataAtualizacao.ToString("yyyy-MM-dd HH:mm:ss")
            };

            await conn.ExecuteAsync(sql, parameters);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();

            string sql = "DELETE FROM Filmes WHERE Id = @Id";
            await conn.ExecuteAsync(sql, new { Id = id });
        }
    }
}
