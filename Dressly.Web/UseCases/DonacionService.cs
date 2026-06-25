using Dressly.Domain.Entities;
using Dressly.Application.Ports.Input;
using Dressly.Application.Ports.Output;

namespace Dressly.Application.UseCases;

public class DonacionService : IDonacionService
{
    private readonly IDonacionRepository _donaciones;
    private readonly IPrendaService _prendas;

    public DonacionService(IDonacionRepository donaciones, IPrendaService prendas)
    {
        _donaciones = donaciones;
        _prendas = prendas;
    }

    public Task<List<LoteDonacion>> GetLotesAsync(int usuarioId)
        => _donaciones.GetLotesByUsuarioIdAsync(usuarioId);

    public Task<LoteDonacion?> GetLoteByIdAsync(int id)
        => _donaciones.GetLoteByIdAsync(id);

    public Task<List<PuntoONG>> GetPuntosONGAsync()
        => _donaciones.GetPuntosONGAsync();

    public async Task RegistrarDonacionAsync(int usuarioId, List<int> prendaIds, int puntoONGId)
    {
        var lote = new LoteDonacion
        {
            PrendaIds = prendaIds,
            PuntoONGId = puntoONGId,
            UsuarioId = usuarioId,
            Estado = "Pendiente",
            FechaCreacion = DateTime.Now
        };

        await _donaciones.AddLoteAsync(lote);
        await _prendas.MarcarEnDesusoAsync(prendaIds);
    }

    public async Task QuitarPrendaDelLoteAsync(int loteId, int prendaId)
    {
        var lote = await _donaciones.GetLoteByIdAsync(loteId);
        if (lote == null) return;

        await _donaciones.RemovePrendaFromLoteAsync(loteId, prendaId);
        await _prendas.DesmarcarEnDesusoAsync(prendaId);

        if (!lote.PrendaIds.Any())
            await _donaciones.DeleteLoteAsync(loteId);
    }

    public async Task CancelarLoteAsync(int loteId)
    {
        var lote = await _donaciones.GetLoteByIdAsync(loteId);
        if (lote == null) return;

        foreach (var prendaId in lote.PrendaIds.ToList())
            await _prendas.DesmarcarEnDesusoAsync(prendaId);

        await _donaciones.DeleteLoteAsync(loteId);
    }

    public async Task MarcarEntregadoAsync(int loteId)
    {
        var lote = await _donaciones.GetLoteByIdAsync(loteId);
        if (lote == null || lote.Estado != "Pendiente") return;

        lote.Estado = "Entregado";
        await _donaciones.UpdateLoteAsync(lote);
    }
}
