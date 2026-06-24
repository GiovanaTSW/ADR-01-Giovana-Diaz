using Dressly_MVC.Models;

namespace Dressly_MVC.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly JsonRepository<Usuario> _repo;

    public UsuarioRepository()
    {
        _repo = new JsonRepository<Usuario>("usuarios.json");
    }

    public Task<List<Usuario>> GetAllAsync() => _repo.GetAllAsync();
    public Task<Usuario?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task<int> GetNextIdAsync() => _repo.GetNextIdAsync();
    public Task AddAsync(Usuario usuario) => _repo.AddAsync(usuario);
    public Task UpdateAsync(Usuario usuario) => _repo.UpdateAsync(usuario);

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _repo.FirstOrDefaultAsync(u => u.Email == email);
    }
}
