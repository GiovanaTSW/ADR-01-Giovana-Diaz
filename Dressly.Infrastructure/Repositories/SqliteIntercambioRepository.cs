using Dressly.Application.Ports.Output;
using Dressly.Domain.Entities;
using Dressly.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Dressly.Infrastructure.Repositories;

public class SqliteIntercambioRepository : IIntercambioRepository
{
    private readonly SqliteDbContext _db;

    public SqliteIntercambioRepository(SqliteDbContext db)
    {
        _db = db;
    }

    public async Task<Intercambio?> GetByIdAsync(int id)
        => await _db.Intercambios.FirstOrDefaultAsync(i => i.Id == id);

    public async Task<List<Intercambio>> GetByUsuarioOfertanteIdAsync(int usuarioId)
        => await _db.Intercambios.AsNoTracking().Where(i => i.UsuarioOfertanteId == usuarioId).ToListAsync();

    public async Task<List<Intercambio>> GetByUsuarioInteresadoIdAsync(int usuarioId)
        => await _db.Intercambios.AsNoTracking().Where(i => i.UsuarioInteresadoId == usuarioId).ToListAsync();

    public async Task<List<Intercambio>> GetPublicadosAsync()
        => await _db.Intercambios.AsNoTracking().Where(i => i.Estado == EstadoIntercambio.Publicado).ToListAsync();

    public async Task AddAsync(Intercambio intercambio)
    {
        _db.Intercambios.Add(intercambio);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Intercambio intercambio)
    {
        var existing = await _db.Intercambios.FindAsync(intercambio.Id);
        if (existing != null)
        {
            _db.Entry(existing).CurrentValues.SetValues(intercambio);
            await _db.SaveChangesAsync();
        }
    }
}
