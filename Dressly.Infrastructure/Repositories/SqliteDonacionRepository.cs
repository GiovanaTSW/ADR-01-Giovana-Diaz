using Dressly.Domain.Entities;
using Dressly.Application.Ports.Output;
using Dressly.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Dressly.Infrastructure.Repositories;

public class SqliteDonacionRepository : IDonacionRepository
{
    private readonly SqliteDbContext _db;

    public SqliteDonacionRepository(SqliteDbContext db)
    {
        _db = db;
    }

    public async Task<List<LoteDonacion>> GetLotesByUsuarioIdAsync(int usuarioId)
        => await _db.LotesDonacion.AsNoTracking().Where(l => l.UsuarioId == usuarioId).ToListAsync();

    public async Task<LoteDonacion?> GetLoteByIdAsync(int id)
        => await _db.LotesDonacion.FirstOrDefaultAsync(l => l.Id == id);

    public async Task<List<PuntoONG>> GetPuntosONGAsync()
        => await _db.PuntosONG.AsNoTracking().ToListAsync();

    public async Task AddPuntoONGAsync(PuntoONG punto)
    {
        _db.PuntosONG.Add(punto);
        await _db.SaveChangesAsync();
    }

    public async Task AddLoteAsync(LoteDonacion lote)
    {
        _db.LotesDonacion.Add(lote);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateLoteAsync(LoteDonacion lote)
    {
        _db.LotesDonacion.Update(lote);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteLoteAsync(int id)
    {
        var lote = await _db.LotesDonacion.FindAsync(id);
        if (lote != null)
        {
            _db.LotesDonacion.Remove(lote);
            await _db.SaveChangesAsync();
        }
    }

    public async Task RemovePrendaFromLoteAsync(int loteId, int prendaId)
    {
        var lote = await _db.LotesDonacion.FindAsync(loteId);
        if (lote == null) return;

        lote.PrendaIds.Remove(prendaId);
        await _db.SaveChangesAsync();
    }
}
