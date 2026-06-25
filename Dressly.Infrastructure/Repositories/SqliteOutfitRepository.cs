using Dressly.Domain.Entities;
using Dressly.Application.Ports.Output;
using Dressly.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Dressly.Infrastructure.Repositories;

public class SqliteOutfitRepository : IOutfitRepository
{
    private readonly SqliteDbContext _db;

    public SqliteOutfitRepository(SqliteDbContext db)
    {
        _db = db;
    }

    public async Task<List<Outfit>> GetAllAsync()
        => await _db.Outfits.AsNoTracking().ToListAsync();

    public async Task<Outfit?> GetByIdAsync(int id)
        => await _db.Outfits.FirstOrDefaultAsync(o => o.Id == id);

    public async Task<List<Outfit>> GetByUsuarioIdAsync(int usuarioId)
        => await _db.Outfits.AsNoTracking().Where(o => o.UsuarioId == usuarioId).ToListAsync();

    public async Task<int> GetNextIdAsync()
    {
        var max = await _db.Outfits.MaxAsync(o => (int?)o.Id) ?? 0;
        return max + 1;
    }

    public async Task AddAsync(Outfit outfit)
    {
        _db.Outfits.Add(outfit);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var outfit = await _db.Outfits.FindAsync(id);
        if (outfit != null)
        {
            _db.Outfits.Remove(outfit);
            await _db.SaveChangesAsync();
        }
    }
}
