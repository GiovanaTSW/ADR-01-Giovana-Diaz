using Dressly_MVC.Models;

namespace Dressly_MVC.Services;

public interface IDonacionService
{
    Task<List<LoteDonacion>> GetLotesAsync(int usuarioId);
    Task<LoteDonacion?> GetLoteByIdAsync(int id);
    Task<List<PuntoONG>> GetPuntosONGAsync();
    Task RegistrarDonacionAsync(int usuarioId, List<int> prendaIds, int puntoONGId);
    Task QuitarPrendaDelLoteAsync(int loteId, int prendaId);
    Task CancelarLoteAsync(int loteId);
    Task MarcarEntregadoAsync(int loteId);
}
