using System.Security.Claims;
using Dressly_MVC.Models;
using Dressly_MVC.Services;
using Dressly_MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dressly_MVC.Controllers;

[Authorize]
public class PrendaController : Controller
{
    private readonly IPrendaService _prendas;
    private readonly IOutfitService _outfits;

    public PrendaController(IPrendaService prendas, IOutfitService outfits)
    {
        _prendas = prendas;
        _outfits = outfits;
    }

    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index()
    {
        var prendas = await _prendas.GetPrendasAsync(UsuarioId);
        return View(prendas);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var prenda = await _prendas.GetByIdAsync(id);
        if (prenda == null) return NotFound();
        return View(prenda);
    }

    public IActionResult Crear() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Prenda prenda, IFormFile? foto)
    {
        if (!ModelState.IsValid) return View(prenda);
        prenda.UsuarioId = UsuarioId;
        await _prendas.CrearAsync(prenda, foto);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _prendas.EliminarAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleEstadoUso(int id, string? returnUrl = null)
    {
        await _prendas.ToggleEstadoUsoAsync(id);
        return Redirect(returnUrl ?? Url.Action(nameof(Index))!);
    }

    public async Task<IActionResult> Estadisticas()
    {
        var prendas = await _prendas.GetPrendasAsync(UsuarioId);
        var outfits = await _outfits.GetOutfitsAsync(UsuarioId);

        var corte = DateTime.Now.AddDays(-90);

        var vm = new EstadisticasViewModel
        {
            TotalPrendas = prendas.Count,
            TotalOutfits = outfits.Count,
            EnDesuso = prendas.Count(p => p.EnDesuso),
            PrendasDonar = prendas.Count(p => p.EnDesuso || p.FechaUltimoUso <= corte),

            TopPrendas = prendas
                .OrderByDescending(p => p.VecesUsada)
                .Take(5)
                .Select(p => new PrendaResumen
                {
                    Nombre = p.Nombre,
                    VecesUsada = p.VecesUsada,
                    FotoUrl = p.FotoUrl
                })
                .ToList(),

            DistribucionColores = prendas
                .GroupBy(p => p.Color)
                .Select(g => new ConteoColor
                {
                    Color = g.Key,
                    Cantidad = g.Count(),
                    HexCode = ObtenerHexColor(g.Key)
                })
                .OrderByDescending(c => c.Cantidad)
                .ToList(),

            DistribucionCategorias = prendas
                .GroupBy(p => p.Categoria)
                .Select(g => new ConteoCategoria
                {
                    Categoria = g.Key,
                    Cantidad = g.Count()
                })
                .OrderByDescending(c => c.Cantidad)
                .ToList(),

            DistribucionTemporadas = prendas
                .GroupBy(p => p.Estacion)
                .Select(g => new ConteoTemporada
                {
                    Temporada = g.Key,
                    Cantidad = g.Count()
                })
                .OrderByDescending(c => c.Cantidad)
                .ToList()
        };

        return View(vm);
    }

    private static string ObtenerHexColor(string color)
    {
        return color?.ToLower() switch
        {
            "negro" => "#212121",
            "blanco" => "#FAFAFA",
            "rojo" => "#B71C1C",
            "azul marino" => "#0D47A1",
            "azul" => "#1565C0",
            "fucsia" => "#AD1457",
            "rosa palo" => "#F8BBD0",
            "rosa" => "#E91E63",
            "lavanda" => "#CE93D8",
            "verde" => "#2E7D32",
            "verde claro" => "#A5D6A7",
            "verde oliva" => "#827717",
            "mostaza" => "#F9A825",
            "dorado" => "#FFC107",
            "naranja" => "#E64A19",
            "coral" => "#FF7043",
            "terracota" => "#BF360C",
            "melocotón" => "#FFAB91",
            "marrón" => "#795548",
            "beige" => "#F5F5DC",
            "gris" => "#9E9E9E",
            "gris perla" => "#ECEFF1",
            "plateado" => "#BDBDBD",
            "crema" => "#FFFDE7",
            _ => "#90A4AE"
        };
    }
}
