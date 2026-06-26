using Dressly.Domain.Entities;
using Dressly.Domain.Events;
using Dressly.Application.Ports.Input;
using Dressly.Application.Ports.Output;

namespace Dressly.Application.UseCases;

public class DonacionService : IDonacionService
{
    private readonly IDonacionRepository _donaciones;
    private readonly IPrendaService _prendas;
    private readonly List<IEventObserver<DonacionRegistradaEvent>> _donacionObservers = new();

    public DonacionService(IDonacionRepository donaciones, IPrendaService prendas)
    {
        _donaciones = donaciones;
        _prendas = prendas;
    }

    public void SubscribeDonacionRegistrada(IEventObserver<DonacionRegistradaEvent> observer)
        => _donacionObservers.Add(observer);

    public Task<List<LoteDonacion>> GetLotesAsync(int usuarioId)
        => _donaciones.GetLotesByUsuarioIdAsync(usuarioId);

    public Task<LoteDonacion?> GetLoteByIdAsync(int id)
        => _donaciones.GetLoteByIdAsync(id);

    public Task<List<PuntoONG>> GetPuntosONGAsync()
        => _donaciones.GetPuntosONGAsync();

    public async Task<PuntoONG> AgregarPuntoONGAsync(string nombre, string direccion, string telefono, double? latitud, double? longitud)
    {
        var punto = new PuntoONG
        {
            Nombre = nombre,
            Direccion = direccion,
            Telefono = telefono,
            Latitud = latitud ?? 0,
            Longitud = longitud ?? 0
        };
        await _donaciones.AddPuntoONGAsync(punto);
        return punto;
    }

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

        var evento = new DonacionRegistradaEvent(usuarioId, lote.Id, prendaIds.Count, DateTime.Now);
        foreach (var obs in _donacionObservers)
            await obs.HandleAsync(evento);
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
