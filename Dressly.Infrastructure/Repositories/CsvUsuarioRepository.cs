using Dressly.Domain.Entities;
using Dressly.Application.Ports.Output;

namespace Dressly.Infrastructure.Repositories;

public class CsvUsuarioRepository : IUsuarioRepository
{
    private readonly CsvRepository<Usuario> _repo;

    public CsvUsuarioRepository()
    {
        _repo = new CsvRepository<Usuario>("usuarios.csv");
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
