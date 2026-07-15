using Dressly.Domain.Entities;

namespace Dressly.Application.Ports.Input;

public interface IIntercambioService
{
    Task<Intercambio> PublicarAsync(int usuarioOfertanteId, int prendaOfertadaId);
    Task ProponerAsync(int intercambioId, int usuarioInteresadoId, int prendaInteresadoId);
    Task AceptarAsync(int intercambioId);
    Task RechazarAsync(int intercambioId);
    Task CompletarAsync(int intercambioId);
    Task<List<Intercambio>> ListarPublicadosAsync();
    Task<List<Intercambio>> ListarPorUsuarioAsync(int usuarioId);
}
