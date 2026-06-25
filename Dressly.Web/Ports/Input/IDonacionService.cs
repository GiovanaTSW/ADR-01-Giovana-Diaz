using Dressly.Domain.Entities;

namespace Dressly.Application.Ports.Input;

public interface IDonacionService
{
    Task<List<LoteDonacion>> GetLotesAsync(int usuarioId);
    Task<LoteDonacion?> GetLoteByIdAsync(int id);
    Task<List<PuntoONG>> GetPuntosONGAsync();
    Task<PuntoONG> AgregarPuntoONGAsync(string nombre, string direccion, string telefono, double? latitud, double? longitud);
    Task RegistrarDonacionAsync(int usuarioId, List<int> prendaIds, int puntoONGId);
    Task QuitarPrendaDelLoteAsync(int loteId, int prendaId);
    Task CancelarLoteAsync(int loteId);
    Task MarcarEntregadoAsync(int loteId);
}
