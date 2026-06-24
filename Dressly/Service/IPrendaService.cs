using Dressly_MVC.Models;

namespace Dressly_MVC.Services;

public interface IPrendaService
{
    Task<List<Prenda>> GetPrendasAsync(int usuarioId);
    Task<Prenda?> GetByIdAsync(int id);
    Task<Prenda> CrearAsync(Prenda prenda, IFormFile? foto);
    Task<List<Prenda>> GetDisponiblesAsync(int usuarioId);
    Task<List<Prenda>> GetDisponiblesParaDonarAsync(int usuarioId);
    Task EliminarAsync(int id);
    Task ToggleEstadoUsoAsync(int id);
    Task MarcarEnDesusoAsync(List<int> prendaIds);
    Task DesmarcarEnDesusoAsync(int prendaId);
}
