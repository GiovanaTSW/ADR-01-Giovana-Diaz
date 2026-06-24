using System.Security.Claims;
using Dressly_MVC.Models;
using Dressly_MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dressly_MVC.Controllers;

[Authorize]
public class OutfitController : Controller
{
    private readonly IOutfitService _outfits;

    public OutfitController(IOutfitService outfits)
    {
        _outfits = outfits;
    }

    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index()
    {
        var outfits = await _outfits.GetOutfitsAsync(UsuarioId);
        return View(outfits);
    }

    public IActionResult Generar() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generar(string ocasion)
    {
        var sugerencia = await _outfits.GenerarSugerenciaAsync(UsuarioId, ocasion);
        ViewBag.Ocasion = ocasion;
        ViewBag.EsSugerencia = true;
        return View("Ver", sugerencia);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Guardar(string nombre, string ocasion, List<int> prendaIds)
    {
        if (string.IsNullOrWhiteSpace(nombre) || !prendaIds.Any())
            return RedirectToAction(nameof(Generar));

        await _outfits.GuardarOutfitAsync(UsuarioId, nombre, ocasion, prendaIds);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Ver(int id)
    {
        var outfit = await _outfits.GetByIdAsync(id);
        if (outfit == null) return NotFound();

        var prendas = await _outfits.GetPrendasByOutfitAsync(outfit);
        ViewBag.OutfitNombre = outfit.Nombre;
        ViewBag.OutfitOcasion = outfit.Ocasion;
        ViewBag.OutfitFecha = outfit.FechaCreacion;
        ViewBag.EsSugerencia = false;
        return View("Ver", prendas);
    }
}
