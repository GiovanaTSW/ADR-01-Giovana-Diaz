using Dressly.Application.Ports.Input;
using Dressly.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Dressly.Api.Controllers;

public record CreatePrendaRequest(
    string Nombre, string Categoria, string Color,
    string Talla, string Estacion, int UsuarioId);

public record MarcarDesusoRequest(List<int> PrendaIds);

[ApiController]
[Route("api/[controller]")]
public class PrendaController : ControllerBase
{
    private readonly IPrendaService _prendaService;

    public PrendaController(IPrendaService prendaService)
    {
        _prendaService = prendaService;
    }

    [HttpGet("usuario/{usuarioId}")]
    public async Task<IActionResult> GetPrendasByUser(int usuarioId)
        => Ok(await _prendaService.GetPrendasAsync(usuarioId));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var prenda = await _prendaService.GetByIdAsync(id);
        return prenda is null ? NotFound() : Ok(prenda);
    }

    [HttpGet("usuario/{usuarioId}/disponibles")]
    public async Task<IActionResult> GetDisponibles(int usuarioId)
        => Ok(await _prendaService.GetDisponiblesAsync(usuarioId));

    [HttpGet("usuario/{usuarioId}/para-donar")]
    public async Task<IActionResult> GetParaDonar(int usuarioId)
        => Ok(await _prendaService.GetDisponiblesParaDonarAsync(usuarioId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePrendaRequest request)
    {
        var prenda = new Prenda
        {
            Nombre = request.Nombre,
            Categoria = request.Categoria,
            Color = request.Color,
            Talla = request.Talla,
            Estacion = request.Estacion,
            UsuarioId = request.UsuarioId
        };

        var creada = await _prendaService.CrearAsync(prenda, null, null);
        return CreatedAtAction(nameof(GetById), new { id = creada.Id }, creada);
    }

    [HttpPut("{id}/toggle-uso")]
    public async Task<IActionResult> ToggleEstadoUso(int id)
    {
        await _prendaService.ToggleEstadoUsoAsync(id);
        return NoContent();
    }

    [HttpPut("marcar-desuso")]
    public async Task<IActionResult> MarcarEnDesuso([FromBody] MarcarDesusoRequest request)
    {
        await _prendaService.MarcarEnDesusoAsync(request.PrendaIds);
        return NoContent();
    }

    [HttpPut("{id}/desmarcar-desuso")]
    public async Task<IActionResult> DesmarcarEnDesuso(int id)
    {
        await _prendaService.DesmarcarEnDesusoAsync(id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _prendaService.EliminarAsync(id);
        return NoContent();
    }
}
