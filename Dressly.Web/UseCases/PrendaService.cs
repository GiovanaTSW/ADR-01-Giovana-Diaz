using Dressly.Domain.Entities;
using Dressly.Domain.Events;
using Dressly.Application.Ports.Input;
using Dressly.Application.Ports.Output;

namespace Dressly.Application.UseCases;

public class PrendaService : IPrendaService
{
    private readonly IPrendaRepository _prendas;
    private readonly IAlmacenamientoImagenes _fotos;
    private readonly List<IEventObserver<PrendaCreadaEvent>> _prendaCreadaObservers = new();

    public PrendaService(IPrendaRepository prendas, IAlmacenamientoImagenes fotos)
    {
        _prendas = prendas;
        _fotos = fotos;
    }

    public void SubscribePrendaCreada(IEventObserver<PrendaCreadaEvent> observer)
        => _prendaCreadaObservers.Add(observer);

    public Task<List<Prenda>> GetPrendasAsync(int usuarioId)
        => _prendas.GetByUsuarioIdAsync(usuarioId);

    public Task<Prenda?> GetByIdAsync(int id)
        => _prendas.GetByIdAsync(id);

    public async Task<Prenda> CrearAsync(Prenda prenda, byte[]? fotoBytes, string? fotoNombre)
    {
        prenda.Id = await _prendas.GetNextIdAsync();
        prenda.FechaUltimoUso = DateTime.Now;

        if (fotoBytes != null && fotoBytes.Length > 0 && !string.IsNullOrEmpty(fotoNombre))
            prenda.FotoUrl = await _fotos.GuardarAsync(fotoBytes, fotoNombre);

        await _prendas.AddAsync(prenda);

        var evento = new PrendaCreadaEvent(prenda.UsuarioId, prenda.Id, prenda.Nombre, DateTime.Now);
        foreach (var obs in _prendaCreadaObservers)
            await obs.HandleAsync(evento);

        return prenda;
    }

    public Task<List<Prenda>> GetDisponiblesAsync(int usuarioId)
        => _prendas.GetDisponiblesAsync(usuarioId);

    public Task<List<Prenda>> GetDisponiblesParaDonarAsync(int usuarioId)
        => _prendas.GetDisponiblesParaDonarAsync(usuarioId);

    public async Task EliminarAsync(int id)
    {
        var prenda = await _prendas.GetByIdAsync(id);
        if (prenda != null)
        {
            await _fotos.EliminarAsync(prenda.FotoUrl);
            await _prendas.DeleteAsync(id);
        }
    }

    public async Task ToggleEstadoUsoAsync(int id)
    {
        var prenda = await _prendas.GetByIdAsync(id);
        if (prenda == null) return;

        prenda.EnDesuso = !prenda.EnDesuso;
        if (!prenda.EnDesuso)
            prenda.FechaUltimoUso = DateTime.Now;

        var todas = await _prendas.GetAllAsync();
        var index = todas.FindIndex(p => p.Id == id);
        if (index >= 0) todas[index] = prenda;
        await _prendas.SaveAsync(todas);
    }

    public async Task MarcarEnDesusoAsync(List<int> prendaIds)
    {
        var todas = await _prendas.GetAllAsync();
        foreach (var prenda in todas.Where(p => prendaIds.Contains(p.Id)))
            prenda.EnDesuso = true;
        await _prendas.SaveAsync(todas);
    }

    public async Task DesmarcarEnDesusoAsync(int prendaId)
    {
        var todas = await _prendas.GetAllAsync();
        var prenda = todas.FirstOrDefault(p => p.Id == prendaId);
        if (prenda == null) return;
        prenda.EnDesuso = false;
        await _prendas.SaveAsync(todas);
    }
}
