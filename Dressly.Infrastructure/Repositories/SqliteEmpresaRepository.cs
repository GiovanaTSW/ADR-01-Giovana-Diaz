using Dressly.Application.Ports.Output;
using Dressly.Domain.Entities;
using Dressly.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Dressly.Infrastructure.Repositories;

public class SqliteEmpresaRepository : IEmpresaRepository
{
    private readonly SqliteDbContext _db;

    public SqliteEmpresaRepository(SqliteDbContext db)
    {
        _db = db;
    }

    public async Task<Empresa?> GetByIdAsync(int id)
        => await _db.Empresas.FirstOrDefaultAsync(e => e.Id == id);

    public async Task<List<Empresa>> GetAllAsync()
        => await _db.Empresas.AsNoTracking().ToListAsync();

    public async Task AddAsync(Empresa empresa)
    {
        _db.Empresas.Add(empresa);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Empresa empresa)
    {
        var existing = await _db.Empresas.FindAsync(empresa.Id);
        if (existing != null)
        {
            _db.Entry(existing).CurrentValues.SetValues(empresa);
            await _db.SaveChangesAsync();
        }
    }
}
