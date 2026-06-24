using Dressly_MVC.Models;

namespace Dressly_MVC.Repositories;

public class PrendaRepository : IPrendaRepository
{
    private readonly JsonRepository<Prenda> _repo;

    public PrendaRepository()
    {
        _repo = new JsonRepository<Prenda>("prendas.json");
    }

    public Task<List<Prenda>> GetAllAsync() => _repo.GetAllAsync();
    public Task<Prenda?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task<int> GetNextIdAsync() => _repo.GetNextIdAsync();
    public Task AddAsync(Prenda prenda) => _repo.AddAsync(prenda);
    public Task DeleteAsync(int id) => _repo.DeleteAsync(id);

    public async Task<List<Prenda>> GetByUsuarioIdAsync(int usuarioId)
    {
        var items = await _repo.GetAllAsync();
        return items.Where(p => p.UsuarioId == usuarioId).ToList();
    }

    public async Task<List<Prenda>> GetDisponiblesAsync(int usuarioId)
    {
        var items = await _repo.GetAllAsync();
        return items.Where(p => p.UsuarioId == usuarioId && !p.EnDesuso).ToList();
    }

    public async Task<List<Prenda>> GetDisponiblesParaDonarAsync(int usuarioId)
    {
        var items = await _repo.GetAllAsync();
        var corte = DateTime.Now.AddDays(-90);
        return items.Where(p =>
            p.UsuarioId == usuarioId &&
            (p.EnDesuso || p.FechaUltimoUso <= corte)).ToList();
    }

    public async Task SaveAsync(List<Prenda> prendas)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(prendas,
            new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(
            Path.Combine(Directory.GetCurrentDirectory(), "data", "prendas.json"), json);
    }
}
