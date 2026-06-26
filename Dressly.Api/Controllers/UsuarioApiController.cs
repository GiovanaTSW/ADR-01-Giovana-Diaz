using Dressly.Application.Ports.Input;
using Microsoft.AspNetCore.Mvc;

namespace Dressly.Api.Controllers;

[ApiController]
[Route("api/usuario")]
public class UsuarioApiController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuarioApiController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var usuario = await _usuarioService.GetByIdAsync(id);
        return usuario is null ? NotFound() : Ok(usuario);
    }
}
