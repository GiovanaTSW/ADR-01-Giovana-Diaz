using Dressly.Domain.Entities;

namespace Dressly.Application.Ports.Output;

public interface IIntercambioRepository
{
    Task<Intercambio?> GetByIdAsync(int id);
    Task<List<Intercambio>> GetByUsuarioOfertanteIdAsync(int usuarioId);
    Task<List<Intercambio>> GetByUsuarioInteresadoIdAsync(int usuarioId);
    Task<List<Intercambio>> GetPublicadosAsync();
    Task AddAsync(Intercambio intercambio);
    Task UpdateAsync(Intercambio intercambio);
}
