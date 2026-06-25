using System.Security.Claims;
using Dressly.Domain.Entities;
using Dressly.Application.Ports.Input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dressly_MVC.Controllers;

[Authorize]
public class DonacionController : Controller
{
    private readonly IDonacionService _donaciones;
    private readonly IPrendaService _prendas;

    public DonacionController(IDonacionService donaciones, IPrendaService prendas)
    {
        _donaciones = donaciones;
        _prendas = prendas;
    }

    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index()
    {
        var lotes = await _donaciones.GetLotesAsync(UsuarioId);
        var puntos = await _donaciones.GetPuntosONGAsync();
        ViewBag.Puntos = puntos;
        return View(lotes);
    }

    public async Task<IActionResult> Lote()
    {
        var enDesuso = await _prendas.GetDisponiblesParaDonarAsync(UsuarioId);
        var puntos = await _donaciones.GetPuntosONGAsync();
        ViewBag.Puntos = puntos;
        return View(enDesuso);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AgregarPuntoONG(string nombre, string direccion, string telefono, double? latitud, double? longitud)
    {
        await _donaciones.AgregarPuntoONGAsync(nombre, direccion, telefono, latitud, longitud);
        TempData["PuntoONGCreado"] = true;
        return RedirectToAction(nameof(Lote));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Lote(List<int> prendaIds, int puntoONGId)
    {
        if (!prendaIds.Any())
            return RedirectToAction(nameof(Lote));

        await _donaciones.RegistrarDonacionAsync(UsuarioId, prendaIds, puntoONGId);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var lote = await _donaciones.GetLoteByIdAsync(id);
        if (lote == null || lote.UsuarioId != UsuarioId)
            return NotFound();

        var puntos = await _donaciones.GetPuntosONGAsync();
        var punto = puntos.FirstOrDefault(p => p.Id == lote.PuntoONGId);

        var prendas = new List<Prenda>();
        foreach (var pid in lote.PrendaIds)
        {
            var p = await _prendas.GetByIdAsync(pid);
            if (p != null) prendas.Add(p);
        }

        ViewBag.Punto = punto;
        return View(Tuple.Create(lote, prendas));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> QuitarPrenda(int loteId, int prendaId)
    {
        await _donaciones.QuitarPrendaDelLoteAsync(loteId, prendaId);
        return RedirectToAction(nameof(Detalle), new { id = loteId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelarLote(int loteId)
    {
        await _donaciones.CancelarLoteAsync(loteId);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarcarEntregado(int loteId)
    {
        await _donaciones.MarcarEntregadoAsync(loteId);
        return RedirectToAction(nameof(Detalle), new { id = loteId });
    }
}
