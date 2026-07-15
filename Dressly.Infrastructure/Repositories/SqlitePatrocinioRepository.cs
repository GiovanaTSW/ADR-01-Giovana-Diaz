using Dressly.Application.Ports.Output;
using Dressly.Domain.Entities;
using Dressly.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Dressly.Infrastructure.Repositories;

public class SqlitePatrocinioRepository : IPatrocinioRepository
{
    private readonly SqliteDbContext _db;

    public SqlitePatrocinioRepository(SqliteDbContext db)
    {
        _db = db;
    }

    public async Task<Patrocinio?> GetByIdAsync(int id)
        => await _db.Patrocinios.FirstOrDefaultAsync(p => p.Id == id);

    public async Task<List<Patrocinio>> GetByEmpresaIdAsync(int empresaId)
        => await _db.Patrocinios.AsNoTracking().Where(p => p.EmpresaId == empresaId).ToListAsync();

    public async Task<List<Patrocinio>> GetByPuntoONGIdAsync(int puntoONGId)
        => await _db.Patrocinios.AsNoTracking().Where(p => p.PuntoONGId == puntoONGId).ToListAsync();

    public async Task AddAsync(Patrocinio patrocinio)
    {
        _db.Patrocinios.Add(patrocinio);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Patrocinio patrocinio)
    {
        var existing = await _db.Patrocinios.FindAsync(patrocinio.Id);
        if (existing != null)
        {
            _db.Entry(existing).CurrentValues.SetValues(patrocinio);
            await _db.SaveChangesAsync();
        }
    }
}
