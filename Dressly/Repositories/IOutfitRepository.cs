using Dressly_MVC.Models;

namespace Dressly_MVC.Repositories;

public interface IOutfitRepository
{
    Task<List<Outfit>> GetAllAsync();
    Task<Outfit?> GetByIdAsync(int id);
    Task<List<Outfit>> GetByUsuarioIdAsync(int usuarioId);
    Task<int> GetNextIdAsync();
    Task AddAsync(Outfit outfit);
    Task DeleteAsync(int id);
}
