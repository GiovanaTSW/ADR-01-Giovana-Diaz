using Dressly.Application.Ports.Output;
using Dressly.Domain.Entities;
using Dressly.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Dressly.Infrastructure.Repositories;

public class SqliteNegocioPacaRepository : INegocioPacaRepository
{
    private readonly SqliteDbContext _db;

    public SqliteNegocioPacaRepository(SqliteDbContext db)
    {
        _db = db;
    }

    public async Task<NegocioPaca?> GetByIdAsync(int id)
        => await _db.NegociosPaca.FirstOrDefaultAsync(n => n.Id == id);

    public async Task<IEnumerable<NegocioPaca>> GetAllAsync()
        => await _db.NegociosPaca.AsNoTracking().ToListAsync();

    public async Task<IEnumerable<NegocioPaca>> GetCercanosACategoriaAsync(string categoria, string coordenadasUsuario)
        => await _db.NegociosPaca
            .AsNoTracking()
            .Where(n => n.CategoriaPrenda == categoria)
            .ToListAsync();

    public async Task AddAsync(NegocioPaca negocio)
    {
        _db.NegociosPaca.Add(negocio);
        await _db.SaveChangesAsync();
    }
}
