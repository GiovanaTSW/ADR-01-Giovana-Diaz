using Dressly.Domain.Entities;
using Dressly.Application.Ports.Output;
using Dressly.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Dressly.Infrastructure.Repositories;

public class SqlitePrendaRepository : IPrendaRepository
{
    private readonly SqliteDbContext _db;

    public SqlitePrendaRepository(SqliteDbContext db)
    {
        _db = db;
    }

    public async Task<List<Prenda>> GetAllAsync()
        => await _db.Prendas.AsNoTracking().ToListAsync();

    public async Task<Prenda?> GetByIdAsync(int id)
        => await _db.Prendas.FirstOrDefaultAsync(p => p.Id == id);

    public async Task<List<Prenda>> GetByUsuarioIdAsync(int usuarioId)
        => await _db.Prendas.AsNoTracking().Where(p => p.UsuarioId == usuarioId).ToListAsync();

    public async Task<List<Prenda>> GetDisponiblesAsync(int usuarioId)
        => await _db.Prendas.AsNoTracking().Where(p => p.UsuarioId == usuarioId && !p.EnDesuso && !p.EsDonada).ToListAsync();

    public async Task<List<Prenda>> GetDisponiblesParaDonarAsync(int usuarioId)
    {
        var corte = DateTime.Now.AddDays(-90);
        return await _db.Prendas.AsNoTracking()
            .Where(p => p.UsuarioId == usuarioId && !p.EsDonada && (p.EnDesuso || p.FechaUltimoUso <= corte))
            .ToListAsync();
    }

    public async Task<int> GetNextIdAsync()
    {
        var max = await _db.Prendas.MaxAsync(p => (int?)p.Id) ?? 0;
        return max + 1;
    }

    public async Task AddAsync(Prenda prenda)
    {
        _db.Prendas.Add(prenda);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var prenda = await _db.Prendas.FindAsync(id);
        if (prenda != null)
        {
            _db.Prendas.Remove(prenda);
            await _db.SaveChangesAsync();
        }
    }

    public async Task SaveAsync(List<Prenda> prendas)
    {
        foreach (var prenda in prendas)
        {
            var existing = await _db.Prendas.FindAsync(prenda.Id);
            if (existing != null)
                _db.Entry(existing).CurrentValues.SetValues(prenda);
            else
                _db.Prendas.Add(prenda);
        }
        await _db.SaveChangesAsync();
    }
}
