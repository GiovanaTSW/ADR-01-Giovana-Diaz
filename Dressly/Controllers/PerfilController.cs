using System.Security.Claims;
using Dressly.Domain.Entities;
using Dressly.Domain.DomainServices;
using Dressly.Application.Ports.Input;
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

    public PerfilController(
        IPerfilService perfil,
        IPerfilConocimientoService conocimiento,
        IAlmacenamientoImagenes fotos)
    {
        _perfil = perfil;
        _conocimiento = conocimiento;
        _fotos = fotos;
    }

    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index()
    {
        var perfil = await _perfil.GetPerfilAsync(UsuarioId);

        if (perfil == null || string.IsNullOrEmpty(perfil.TipoCuerpo))
            return RedirectToAction(nameof(Editar));

        var vm = new PerfilViewModel
        {
            Perfil = perfil,
            TipoCuerpoInfo = _conocimiento.ObtenerInfoTipoCuerpo(perfil?.TipoCuerpo),
            ColorimetriaInfo = _conocimiento.ObtenerInfoColorimetria(perfil?.Colorimetria),
            ContrasteInfo = _conocimiento.ObtenerInfoContraste(perfil?.Contraste)
        };

        return View(vm);
    }

    public async Task<IActionResult> Editar()
    {
        var perfil = await _perfil.GetPerfilAsync(UsuarioId) ?? new PerfilFisico();
        DetectarYAsignarEstacion(perfil);
        return View(perfil);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(PerfilFisico perfil, IFormFile? foto)
    {
        if (!ModelState.IsValid)
        {
            DetectarYAsignarEstacion(perfil);
            return View(perfil);
        }

        if (string.IsNullOrEmpty(perfil.Colorimetria))
        {
            var detectada = _conocimiento.DetectarEstacion(
                perfil.SubtonoPiel, perfil.IntensidadCabello, perfil.ColorOjos);
            if (!string.IsNullOrEmpty(detectada))
                perfil.Colorimetria = detectada;
        }

        var existente = await _perfil.GetPerfilAsync(UsuarioId);

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

        perfil.UsuarioId = UsuarioId;
        perfil.Id = existente?.Id ?? UsuarioId;

        ModelState.Clear();
        await _perfil.GuardarPerfilAsync(UsuarioId, perfil);
        return RedirectToAction(nameof(Index));
    }

    private void DetectarYAsignarEstacion(PerfilFisico perfil)
    {
        if (!string.IsNullOrEmpty(perfil.SubtonoPiel) &&
            !string.IsNullOrEmpty(perfil.IntensidadCabello) &&
            string.IsNullOrEmpty(perfil.Colorimetria))
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
}
