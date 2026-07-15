using Dressly.Domain.Entities;
using Dressly.Application.Ports.Output;
using Dressly.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Dressly.Infrastructure.Repositories;

public class SqliteUsuarioRepository : IUsuarioRepository
{
    private readonly SqliteDbContext _db;

    public SqliteUsuarioRepository(SqliteDbContext db)
    {
        _db = db;
    }

    public async Task<List<Usuario>> GetAllAsync()
        => await _db.Usuarios.AsNoTracking().ToListAsync();

    public async Task<Usuario?> GetByIdAsync(int id)
        => await _db.Usuarios
        .Include(u => u.Perfil)
            .ThenInclude(p => p.KibbeInfo)
        .FirstOrDefaultAsync(u => u.Id == id);

    public async Task<Usuario?> GetByEmailAsync(string email)
        => await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<int> GetNextIdAsync()
    {
        var max = await _db.Usuarios.MaxAsync(u => (int?)u.Id) ?? 0;
        return max + 1;
    }

    public async Task AddAsync(Usuario usuario)
    {
        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Usuario usuario)
    {
        _db.Usuarios.Update(usuario);
        await _db.SaveChangesAsync();
    }
}
