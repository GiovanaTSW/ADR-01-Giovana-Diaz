using Dressly.Application.Ports.Input;
using Dressly.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Dressly.Api.Controllers;

[ApiController]
[Route("api/perfil")]
public class PerfilApiController : ControllerBase
{
    private readonly IPerfilService _perfilService;

    public PerfilApiController(IPerfilService perfilService)
    {
        _perfilService = perfilService;
    }

    [HttpGet("{usuarioId}")]
    public async Task<IActionResult> GetPerfil(int usuarioId)
    {
        var perfil = await _perfilService.GetPerfilAsync(usuarioId);
        return perfil is null ? NotFound() : Ok(perfil);
    }

    [HttpPost("{usuarioId}")]
    public async Task<IActionResult> GuardarPerfil(int usuarioId, [FromBody] PerfilFisico perfil)
    {
        await _perfilService.GuardarPerfilAsync(usuarioId, perfil);
        return Ok(perfil);
    }
}
