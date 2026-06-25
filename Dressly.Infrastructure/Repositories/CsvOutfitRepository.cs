using Dressly.Domain.Entities;
using Dressly.Application.Ports.Output;

namespace Dressly.Infrastructure.Repositories;

public class CsvOutfitRepository : IOutfitRepository
{
    private readonly CsvRepository<Outfit> _repo;

    public CsvOutfitRepository()
    {
        _repo = new CsvRepository<Outfit>("outfits.csv");
    }

    public Task<List<Outfit>> GetAllAsync() => _repo.GetAllAsync();
    public Task<Outfit?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task<int> GetNextIdAsync() => _repo.GetNextIdAsync();
    public Task AddAsync(Outfit outfit) => _repo.AddAsync(outfit);
    public Task DeleteAsync(int id) => _repo.DeleteAsync(id);

    public async Task<List<Outfit>> GetByUsuarioIdAsync(int usuarioId)
        => await _repo.FindAsync(o => o.UsuarioId == usuarioId);
}
