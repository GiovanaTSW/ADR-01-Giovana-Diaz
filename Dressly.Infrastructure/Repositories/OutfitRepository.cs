using Dressly.Domain.Entities;
using Dressly.Application.Ports.Output;

namespace Dressly_MVC.Repositories;

public class OutfitRepository : IOutfitRepository
{
    private readonly JsonRepository<Outfit> _repo;

    public OutfitRepository()
    {
        _repo = new JsonRepository<Outfit>("outfits.json");
    }

    public Task<List<Outfit>> GetAllAsync() => _repo.GetAllAsync();
    public Task<Outfit?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task<int> GetNextIdAsync() => _repo.GetNextIdAsync();
    public Task AddAsync(Outfit outfit) => _repo.AddAsync(outfit);
    public Task DeleteAsync(int id) => _repo.DeleteAsync(id);

    public async Task<List<Outfit>> GetByUsuarioIdAsync(int usuarioId)
    {
        return await _repo.FindAsync(o => o.UsuarioId == usuarioId);
    }
}
