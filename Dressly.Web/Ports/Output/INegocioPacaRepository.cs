using Dressly.Domain.Entities;

namespace Dressly.Application.Ports.Output;

public interface INegocioPacaRepository
{
    Task<NegocioPaca?> GetByIdAsync(int id);
    Task<IEnumerable<NegocioPaca>> GetAllAsync();
    Task<IEnumerable<NegocioPaca>> GetCercanosACategoriaAsync(string categoria, string coordenadasUsuario);
    Task AddAsync(NegocioPaca negocio);
}