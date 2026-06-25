using System.Security.Claims;
using Dressly.Application.Ports.Input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dressly_MVC.Controllers;

[Authorize]
public class UsuarioController : Controller
{
    private readonly IUsuarioService _usuarios;

    public UsuarioController(IUsuarioService usuarios)
    {
        _usuarios = usuarios;
    }

    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Perfil()
    {
        var usuario = await _usuarios.GetByIdAsync(UsuarioId);
        if (usuario == null) return NotFound();
        return View(usuario);
    }
}
