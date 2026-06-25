using Dressly.Domain.Entities;

namespace Dressly.Application.Ports.Output;

public interface IPrendaRepository
{
    Task<List<Prenda>> GetAllAsync();
    Task<Prenda?> GetByIdAsync(int id);
    Task<List<Prenda>> GetByUsuarioIdAsync(int usuarioId);
    Task<List<Prenda>> GetDisponiblesAsync(int usuarioId);
    Task<List<Prenda>> GetDisponiblesParaDonarAsync(int usuarioId);
    Task<int> GetNextIdAsync();
    Task AddAsync(Prenda prenda);
    Task DeleteAsync(int id);
    Task SaveAsync(List<Prenda> prendas);
}
