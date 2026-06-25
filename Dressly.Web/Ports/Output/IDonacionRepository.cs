using Dressly.Domain.Entities;

namespace Dressly.Application.Ports.Output;

public interface IDonacionRepository
{
    Task<List<LoteDonacion>> GetLotesByUsuarioIdAsync(int usuarioId);
    Task<LoteDonacion?> GetLoteByIdAsync(int id);
    Task<List<PuntoONG>> GetPuntosONGAsync();
    Task AddPuntoONGAsync(PuntoONG punto);
    Task AddLoteAsync(LoteDonacion lote);
    Task UpdateLoteAsync(LoteDonacion lote);
    Task DeleteLoteAsync(int id);
    Task RemovePrendaFromLoteAsync(int loteId, int prendaId);
}
