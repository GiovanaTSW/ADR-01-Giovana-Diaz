using Dressly.Domain.Entities;

namespace Dressly.Application.Ports.Input;

public interface IOutfitService
{
    Task<List<Outfit>> GetOutfitsAsync(int usuarioId);
    Task<Outfit?> GetByIdAsync(int id);
    Task<List<Prenda>> GenerarSugerenciaAsync(int usuarioId, string ocasion);
    Task<Outfit> GuardarOutfitAsync(int usuarioId, string nombre, string ocasion, List<int> prendaIds);
    Task<List<Prenda>> GetPrendasByOutfitAsync(Outfit outfit);
    Task EliminarOutfitAsync(int id);
}
