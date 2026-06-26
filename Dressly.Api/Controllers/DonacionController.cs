using Dressly.Application.Ports.Input;
using Dressly.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Dressly.Api.Controllers;

public record AgregarPuntoONGRequest(string Nombre, string Direccion, string Telefono, double? Latitud, double? Longitud);
public record RegistrarDonacionRequest(int UsuarioId, List<int> PrendaIds, int PuntoONGId);

[ApiController]
[Route("api/[controller]")]
public class DonacionController : ControllerBase
{
    private readonly IDonacionService _donacionService;

    public DonacionController(IDonacionService donacionService)
    {
        _donacionService = donacionService;
    }

    [HttpGet("usuario/{usuarioId}")]
    public async Task<IActionResult> GetLotesByUser(int usuarioId)
        => Ok(await _donacionService.GetLotesAsync(usuarioId));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var lote = await _donacionService.GetLoteByIdAsync(id);
        return lote is null ? NotFound() : Ok(lote);
    }

    [HttpGet("puntos-ong")]
    public async Task<IActionResult> GetPuntosONG()
        => Ok(await _donacionService.GetPuntosONGAsync());

    [HttpPost("puntos-ong")]
    public async Task<IActionResult> AgregarPuntoONG([FromBody] AgregarPuntoONGRequest request)
    {
        var punto = await _donacionService.AgregarPuntoONGAsync(
            request.Nombre, request.Direccion, request.Telefono, request.Latitud, request.Longitud);
        return CreatedAtAction(nameof(GetPuntosONG), punto);
    }

    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] RegistrarDonacionRequest request)
    {
        await _donacionService.RegistrarDonacionAsync(
            request.UsuarioId, request.PrendaIds, request.PuntoONGId);
        return Ok();
    }

    [HttpPut("{loteId}/quitar-prenda/{prendaId}")]
    public async Task<IActionResult> QuitarPrenda(int loteId, int prendaId)
    {
        await _donacionService.QuitarPrendaDelLoteAsync(loteId, prendaId);
        return NoContent();
    }

    [HttpPut("{loteId}/cancelar")]
    public async Task<IActionResult> Cancelar(int loteId)
    {
        await _donacionService.CancelarLoteAsync(loteId);
        return NoContent();
    }

    [HttpPut("{loteId}/entregar")]
    public async Task<IActionResult> Entregar(int loteId)
    {
        await _donacionService.MarcarEntregadoAsync(loteId);
        return NoContent();
    }
}
