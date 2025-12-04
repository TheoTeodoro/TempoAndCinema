using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;
using TempoAndCinema.Models;
using TempoAndCinema.Data;

public class FilmeRepository : IFilmeRepository
{
    private readonly string _connectionString = "Data Source=C:\\Users\\lucay\\Desktop\\TempoAndCinema-main\\Data\\catalog.db";
    public async Task<int> CreateAsync(Filme filme)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        string sql = @"
            INSERT INTO Filmes
            (TmdbId, Titulo, TituloOriginal, Sinopse, DataLancamento, Genero, PosterPath, Lingua,
             Duracao, NotaMedia, ElencoPrincipal, CidadeReferencia, Latitude, Longitude,
             DataCriacao, DataAtualizacao)
            VALUES
            (@TmdbId, @Titulo, @TituloOriginal, @Sinopse, @DataLancamento, @Genero, @PosterPath, @Lingua,
             @Duracao, @NotaMedia, @ElencoPrincipal, @CidadeReferencia, @Latitude, @Longitude,
             @DataCriacao, @DataAtualizacao);

            SELECT last_insert_rowid();
        ";

        int id = await connection.ExecuteScalarAsync<int>(sql, filme);
        return id;
    }

    public async Task<Filme?> GetByIdAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        string sql = @"SELECT * FROM Filmes WHERE Id = @Id";

        return await connection.QueryFirstOrDefaultAsync<Filme>(sql, new { Id = id });
    }

    public async Task<IEnumerable<Filme>> ListAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        string sql = @"SELECT * FROM Filmes ORDER BY Id DESC";

        return await connection.QueryAsync<Filme>(sql);
    }

    public async Task UpdateAsync(Filme filme)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        string sql = @"
            UPDATE Filmes SET
                TmdbId = @TmdbId,
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
                DataAtualizacao = @DataAtualizacao
            WHERE Id = @Id
        ";

        await connection.ExecuteAsync(sql, filme);
    }

    public async Task DeleteAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        string sql = @"DELETE FROM Filmes WHERE Id = @Id";

        await connection.ExecuteAsync(sql, new { Id = id });
    }
}
