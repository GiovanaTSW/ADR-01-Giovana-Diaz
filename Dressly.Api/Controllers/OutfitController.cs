using Dressly.Application.Ports.Input;
using Dressly.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Dressly.Api.Controllers;

public record SugerenciaRequest(int UsuarioId, string Ocasion);
public record GuardarOutfitRequest(int UsuarioId, string Nombre, string Ocasion, List<int> PrendaIds);

[ApiController]
[Route("api/[controller]")]
public class OutfitController : ControllerBase
{
    private readonly IOutfitService _outfitService;

    public OutfitController(IOutfitService outfitService)
    {
        _outfitService = outfitService;
    }

    [HttpGet("usuario/{usuarioId}")]
    public async Task<IActionResult> GetOutfitsByUser(int usuarioId)
        => Ok(await _outfitService.GetOutfitsAsync(usuarioId));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var outfit = await _outfitService.GetByIdAsync(id);
        return outfit is null ? NotFound() : Ok(outfit);
    }

    [HttpGet("{id}/prendas")]
    public async Task<IActionResult> GetPrendasByOutfit(int id)
    {
        var outfit = await _outfitService.GetByIdAsync(id);
        if (outfit is null) return NotFound();
        return Ok(await _outfitService.GetPrendasByOutfitAsync(outfit));
    }

    [HttpPost("sugerencia")]
    public async Task<IActionResult> GenerarSugerencia([FromBody] SugerenciaRequest request)
        => Ok(await _outfitService.GenerarSugerenciaAsync(request.UsuarioId, request.Ocasion));

    [HttpPost]
    public async Task<IActionResult> Guardar([FromBody] GuardarOutfitRequest request)
    {
        var outfit = await _outfitService.GuardarOutfitAsync(
            request.UsuarioId, request.Nombre, request.Ocasion, request.PrendaIds);
        return CreatedAtAction(nameof(GetById), new { id = outfit.Id }, outfit);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _outfitService.EliminarOutfitAsync(id);
        return NoContent();
    }
}
