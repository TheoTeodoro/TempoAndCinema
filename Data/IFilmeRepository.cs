using TempoAndCinema.Models;

namespace TempoAndCinema.Data
{
    public interface IFilmeRepository
    {
        Task<List<Filme>> GetAllAsync();
        Task<Filme?> GetByIdAsync(int id);
        Task<Filme?> GetByTmdbIdAsync(int tmdbId);
        Task<int> AddAsync(Filme filme);
        Task UpdateAsync(Filme filme);
        Task DeleteAsync(int id);
    }
}