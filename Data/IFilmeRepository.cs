using Microsoft.AspNetCore.Mvc;
namespace TempoAndCinema.Data;
using TempoAndCinema.Models;

public interface IFilmeRepository
{
    Task<int> CreateAsync(Filme filme);
    Task<Filme?> GetByIdAsync(int id);
    Task<IEnumerable<Filme>> ListAsync();
    Task UpdateAsync(Filme filme);
    Task DeleteAsync(int id);
}
