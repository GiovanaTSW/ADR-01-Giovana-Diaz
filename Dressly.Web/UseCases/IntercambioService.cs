using Dressly.Domain.Entities;
using Dressly.Application.Ports.Input;
using Dressly.Application.Ports.Output;
using Microsoft.Extensions.Configuration;

namespace Dressly.Application.UseCases;

public class IntercambioService : IIntercambioService
{
    private readonly IIntercambioRepository _intercambios;
    private readonly IPrendaRepository _prendas;
    private readonly decimal _comision;

    public IntercambioService(
        IIntercambioRepository intercambios,
        IPrendaRepository prendas,
        IConfiguration config)
    {
        _intercambios = intercambios;
        _prendas = prendas;
        var seccion = config.GetSection("Intercambio");
        _comision = decimal.TryParse(seccion["Comision"], out var c) ? c : 5.00m;
    }

    public async Task<Intercambio> PublicarAsync(int usuarioOfertanteId, int prendaOfertadaId)
    {
        var intercambio = new Intercambio
        {
            UsuarioOfertanteId = usuarioOfertanteId,
            PrendaOfertadaId = prendaOfertadaId,
            Estado = EstadoIntercambio.Publicado,
            Comision = _comision
        };
        await _intercambios.AddAsync(intercambio);
        return intercambio;
    }

    public async Task ProponerAsync(int intercambioId, int usuarioInteresadoId, int prendaInteresadoId)
    {
        var intercambio = await _intercambios.GetByIdAsync(intercambioId);
        if (intercambio == null || intercambio.Estado != EstadoIntercambio.Publicado)
            return;

        intercambio.UsuarioInteresadoId = usuarioInteresadoId;
        intercambio.PrendaInteresadoId = prendaInteresadoId;
        intercambio.Estado = EstadoIntercambio.Propuesto;
        await _intercambios.UpdateAsync(intercambio);
    }

    public async Task AceptarAsync(int intercambioId)
    {
        var intercambio = await _intercambios.GetByIdAsync(intercambioId);
        if (intercambio == null || intercambio.Estado != EstadoIntercambio.Propuesto)
            return;

        intercambio.Estado = EstadoIntercambio.Aceptado;
        await _intercambios.UpdateAsync(intercambio);
    }

    public async Task RechazarAsync(int intercambioId)
    {
        var intercambio = await _intercambios.GetByIdAsync(intercambioId);
        if (intercambio == null || intercambio.Estado != EstadoIntercambio.Propuesto)
            return;

        intercambio.Estado = EstadoIntercambio.Rechazado;
        await _intercambios.UpdateAsync(intercambio);
    }

    public async Task CompletarAsync(int intercambioId)
    {
        var intercambio = await _intercambios.GetByIdAsync(intercambioId);
        if (intercambio == null || intercambio.Estado != EstadoIntercambio.Aceptado)
            return;

        var ofertada = await _prendas.GetByIdAsync(intercambio.PrendaOfertadaId);
        var interesada = intercambio.PrendaInteresadoId.HasValue
            ? await _prendas.GetByIdAsync(intercambio.PrendaInteresadoId.Value)
            : null;

        if (ofertada != null)
        {
            ofertada.UsuarioId = intercambio.UsuarioInteresadoId ?? ofertada.UsuarioId;
            ofertada.VecesUsada++;
            ofertada.FechaUltimoUso = DateTime.Now;
        }

        if (interesada != null)
        {
            interesada.UsuarioId = intercambio.UsuarioOfertanteId;
            interesada.VecesUsada++;
            interesada.FechaUltimoUso = DateTime.Now;
        }

        var prendasActualizadas = new List<Prenda>();
        if (ofertada != null) prendasActualizadas.Add(ofertada);
        if (interesada != null) prendasActualizadas.Add(interesada);
        if (prendasActualizadas.Count > 0)
            await _prendas.SaveAsync(prendasActualizadas);

        intercambio.Estado = EstadoIntercambio.Completado;
        await _intercambios.UpdateAsync(intercambio);
    }

    public Task<List<Intercambio>> ListarPublicadosAsync()
        => _intercambios.GetPublicadosAsync();

    public async Task<List<Intercambio>> ListarPorUsuarioAsync(int usuarioId)
    {
        var ofertados = await _intercambios.GetByUsuarioOfertanteIdAsync(usuarioId);
        var interesados = await _intercambios.GetByUsuarioInteresadoIdAsync(usuarioId);
        return ofertados.Concat(interesados).DistinctBy(i => i.Id).ToList();
    }
}
