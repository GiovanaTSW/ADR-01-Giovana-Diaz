using Dressly_MVC.Models;

namespace Dressly_MVC.Repositories;

public interface IDonacionRepository
{
    Task<DonacionData> GetDataAsync();
    Task<List<LoteDonacion>> GetLotesByUsuarioIdAsync(int usuarioId);
    Task<LoteDonacion?> GetLoteByIdAsync(int id);
    Task<List<PuntoONG>> GetPuntosONGAsync();
    Task AddLoteAsync(LoteDonacion lote);
    Task UpdateLoteAsync(LoteDonacion lote);
    Task RemovePrendaFromLoteAsync(int loteId, int prendaId);
    Task DeleteLoteAsync(int loteId);
    Task SaveDataAsync(DonacionData data);
}
