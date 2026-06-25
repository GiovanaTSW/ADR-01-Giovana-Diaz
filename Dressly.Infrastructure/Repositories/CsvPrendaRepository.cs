using Dressly.Domain.Entities;
using Dressly.Application.Ports.Output;

namespace Dressly.Infrastructure.Repositories;

public class CsvPrendaRepository : IPrendaRepository
{
    private readonly CsvRepository<Prenda> _repo;

    public CsvPrendaRepository()
    {
        _repo = new CsvRepository<Prenda>("prendas.csv");
    }

    public Task<List<Prenda>> GetAllAsync() => _repo.GetAllAsync();
    public Task<Prenda?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task<int> GetNextIdAsync() => _repo.GetNextIdAsync();
    public Task AddAsync(Prenda prenda) => _repo.AddAsync(prenda);
    public Task DeleteAsync(int id) => _repo.DeleteAsync(id);

    public async Task<List<Prenda>> GetByUsuarioIdAsync(int usuarioId)
        => await _repo.FindAsync(p => p.UsuarioId == usuarioId);

    public async Task<List<Prenda>> GetDisponiblesAsync(int usuarioId)
        => await _repo.FindAsync(p => p.UsuarioId == usuarioId && !p.EnDesuso);

    public async Task<List<Prenda>> GetDisponiblesParaDonarAsync(int usuarioId)
    {
        var corte = DateTime.Now.AddDays(-90);
        return await _repo.FindAsync(p =>
            p.UsuarioId == usuarioId &&
            (p.EnDesuso || p.FechaUltimoUso <= corte));
    }

    public async Task SaveAsync(List<Prenda> prendas)
    {
        await _repo.SaveAllAsync(prendas);
    }
}
