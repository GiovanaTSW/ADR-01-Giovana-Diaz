using Dressly.Domain.Entities;
using Dressly.Application.Ports.Output;

namespace Dressly.Infrastructure.Repositories;

public class CsvDonacionRepository : IDonacionRepository
{
    private readonly CsvRepository<LoteDonacion> _lotes;
    private readonly CsvRepository<PuntoONG> _puntos;

    public CsvDonacionRepository()
    {
        _lotes = new CsvRepository<LoteDonacion>("lotes.csv");
        _puntos = new CsvRepository<PuntoONG>("puntosong.csv");
    }

    public async Task<List<LoteDonacion>> GetLotesByUsuarioIdAsync(int usuarioId)
        => await _lotes.FindAsync(l => l.UsuarioId == usuarioId);

    public Task<LoteDonacion?> GetLoteByIdAsync(int id)
        => _lotes.GetByIdAsync(id);

    public Task<List<PuntoONG>> GetPuntosONGAsync()
        => _puntos.GetAllAsync();

    public async Task AddPuntoONGAsync(PuntoONG punto)
        => await _puntos.AddAsync(punto);

    public async Task AddLoteAsync(LoteDonacion lote)
        => await _lotes.AddAsync(lote);

    public async Task UpdateLoteAsync(LoteDonacion lote)
        => await _lotes.UpdateAsync(lote);

    public async Task DeleteLoteAsync(int id)
        => await _lotes.DeleteAsync(id);

    public async Task RemovePrendaFromLoteAsync(int loteId, int prendaId)
    {
        var lote = await _lotes.GetByIdAsync(loteId);
        if (lote == null) return;

        lote.PrendaIds.Remove(prendaId);
        await _lotes.UpdateAsync(lote);
    }
}
