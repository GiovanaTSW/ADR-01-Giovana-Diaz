using System.Security.Claims;
using Dressly.Domain.Entities;
using Dressly.Domain.DomainServices;
using Dressly.Application.Ports.Input;
using Dressly.Application.Ports.Output;
using Dressly_MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dressly_MVC.Controllers;

[Authorize]
public class PerfilController : Controller
{
    private readonly IPerfilService _perfil;
    private readonly IPerfilConocimientoService _conocimiento;
    private readonly IAlmacenamientoImagenes _fotos;
    private readonly IIdentidadKibbeRepository _kibbe;

    public PerfilController(
        IPerfilService perfil,
        IPerfilConocimientoService conocimiento,
        IAlmacenamientoImagenes fotos,
        IIdentidadKibbeRepository kibbe)
    {
        _perfil = perfil;
        _conocimiento = conocimiento;
        _fotos = fotos;
        _kibbe = kibbe;
    }

    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index()
    {
        var perfil = await _perfil.GetPerfilAsync(UsuarioId);

        if (perfil == null || perfil.KibbeInfoId is null or 0)
        {
            return RedirectToAction(nameof(Editar));
        }

        var vm = new PerfilViewModel
        {
            Perfil = perfil,
            KibbeInfo = perfil.KibbeInfo,
            KibbeInfoImagen = ObtenerImagenKibbe(perfil.KibbeInfo?.Nombre),
            TipoCuerpoInfo = _conocimiento.ObtenerInfoTipoCuerpo(perfil.TipoCuerpo),
            ColorimetriaInfo = _conocimiento.ObtenerInfoColorimetria(perfil.Colorimetria),
            ContrasteInfo = _conocimiento.ObtenerInfoContraste(perfil.Contraste)
        };

        return View(vm);
    }

    public async Task<IActionResult> Editar()
    {
        var perfil = await _perfil.GetPerfilAsync(UsuarioId) ?? new PerfilFisico();
        DetectarYAsignarEstacion(perfil);
        return View(await BuildEditarVMAsync(perfil));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(PerfilEditarViewModel vm, IFormFile? foto)
    {
        var perfil = vm.Perfil!;
        if (!ModelState.IsValid)
        {
            DetectarYAsignarEstacion(perfil);
            return View(await BuildEditarVMAsync(perfil));
        }

        try
        {
            var existente = await _perfil.GetPerfilAsync(UsuarioId);
            perfil.UsuarioId = UsuarioId;
            perfil.Id = existente?.Id ?? 0;

            if (existente != null)
            {
                perfil.Saturacion = existente.Saturacion;
                if (string.IsNullOrEmpty(perfil.TipoCuerpo))
                    perfil.TipoCuerpo = existente.TipoCuerpo;
                if (perfil.KibbeInfoId is null or 0)
                    perfil.KibbeInfoId = existente.KibbeInfoId;
            }

            // Lógica de foto
            if (foto != null && foto.Length > 0)
            {
                if (existente?.FotoUrl != null)
                    await _fotos.EliminarAsync(existente.FotoUrl);

                using var ms = new MemoryStream();
                await foto.CopyToAsync(ms);
                perfil.FotoUrl = await _fotos.GuardarAsync(ms.ToArray(), foto.FileName);
            }
            else
            {
                perfil.FotoUrl = existente?.FotoUrl;
            }

            // Lógica de detección si falta colorimetría
            if (string.IsNullOrEmpty(perfil.Colorimetria))
            {
                var detectada = _conocimiento.DetectarEstacion(
                    perfil.SubtonoPiel, perfil.IntensidadCabello, perfil.ColorOjos);
                if (!string.IsNullOrEmpty(detectada))
                {
                    perfil.Colorimetria = detectada;
                    ViewBag.EstacionDetectada = detectada;
                }
            }

            await _perfil.GuardarPerfilAsync(UsuarioId, perfil);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "Ocurrió un error al guardar los cambios.");
            return View(await BuildEditarVMAsync(perfil));
        }
    }

    private async Task<PerfilEditarViewModel> BuildEditarVMAsync(PerfilFisico perfil)
    {
        return new PerfilEditarViewModel
        {
            Perfil = perfil,
            Identidades = await _kibbe.GetAllAsync(),
            ImagenesPorNombre = ImagenesKibbe.ToDictionary(
                kv => kv.Key,
                kv => kv.Value,
                StringComparer.OrdinalIgnoreCase)
        };
    }

    private void DetectarYAsignarEstacion(PerfilFisico perfil)
    {
        if (string.IsNullOrEmpty(perfil.Colorimetria))
        {
            var detectada = _conocimiento.DetectarEstacion(
                perfil.SubtonoPiel, perfil.IntensidadCabello, perfil.ColorOjos);
            if (!string.IsNullOrEmpty(detectada))
            {
                perfil.Colorimetria = detectada;
                ViewBag.EstacionDetectada = detectada;
            }
        }
    }

    private static readonly Dictionary<string, string> ImagenesKibbe = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Dramatic", "/img/dramatic c1.png" },
        { "Soft Dramatic", "/img/soft dramatic c2.png" },
        { "Flamboyant Natural", "/img/Flamboyant natural c3.png" },
        { "Soft Natural", "/img/soft natural C4.png" },
        { "Dramatic Classic", "/img/dramatic classic C5.png" },
        { "Soft Classic", "/img/soft classic c6.png" },
        { "Flamboyant Gamine", "/img/flamboyant glamine c7.png" },
        { "Soft Gamine", "/img/Soft gamine c8.png" },
        { "Theatrical Romantic", "/img/theatrical romantic C9.png" },
        { "Romantic", "/img/romantic c10.png" }
    };

    private static string? ObtenerImagenKibbe(string? nombre)
    {
        if (string.IsNullOrEmpty(nombre)) return null;
        if (ImagenesKibbe.TryGetValue(nombre, out var ruta)) return ruta;
        return "/img/tipos.png";
    }

}