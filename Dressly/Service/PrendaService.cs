using Dressly_MVC.Models;
using Dressly_MVC.Repositories;

namespace Dressly_MVC.Services;

public class PrendaService : IPrendaService
{
    private readonly IPrendaRepository _prendas;
    private readonly IFotoService _fotos;

    public PrendaService(IPrendaRepository prendas, IFotoService fotos)
    {
        _prendas = prendas;
        _fotos = fotos;
    }

    public Task<List<Prenda>> GetPrendasAsync(int usuarioId)
        => _prendas.GetByUsuarioIdAsync(usuarioId);

    public Task<Prenda?> GetByIdAsync(int id)
        => _prendas.GetByIdAsync(id);

    public async Task<Prenda> CrearAsync(Prenda prenda, IFormFile? foto)
    {
        prenda.Id = await _prendas.GetNextIdAsync();
        prenda.FechaUltimoUso = DateTime.Now;

        if (foto != null && foto.Length > 0)
            prenda.FotoUrl = await _fotos.GuardarAsync(foto);

        await _prendas.AddAsync(prenda);
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
