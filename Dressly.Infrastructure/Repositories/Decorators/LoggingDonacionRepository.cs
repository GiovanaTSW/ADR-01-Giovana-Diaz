using Dressly.Application.Ports.Output;
using Dressly.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Dressly.Infrastructure.Repositories.Decorators;

public class LoggingDonacionRepository : IDonacionRepository
{
    private readonly IDonacionRepository _inner;
    private readonly ILogger<LoggingDonacionRepository> _logger;

    public LoggingDonacionRepository(IDonacionRepository inner, ILogger<LoggingDonacionRepository> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<List<LoteDonacion>> GetLotesByUsuarioIdAsync(int usuarioId)
    {
        _logger.LogInformation("▶ DonacionRepository.GetLotesByUsuarioIdAsync({UsuarioId})", usuarioId);
        var result = await _inner.GetLotesByUsuarioIdAsync(usuarioId);
        _logger.LogInformation("◀ DonacionRepository.GetLotesByUsuarioIdAsync({UsuarioId}) → {Count} lotes", usuarioId, result.Count);
        return result;
    }

    public async Task<LoteDonacion?> GetLoteByIdAsync(int id)
    {
        _logger.LogInformation("▶ DonacionRepository.GetLoteByIdAsync({Id})", id);
        var result = await _inner.GetLoteByIdAsync(id);
        _logger.LogInformation("◀ DonacionRepository.GetLoteByIdAsync({Id}) → {Found}", id, result != null ? "encontrado" : "null");
        return result;
    }

    public async Task<List<PuntoONG>> GetPuntosONGAsync()
    {
        _logger.LogInformation("▶ DonacionRepository.GetPuntosONGAsync()");
        var result = await _inner.GetPuntosONGAsync();
        _logger.LogInformation("◀ DonacionRepository.GetPuntosONGAsync() → {Count} puntos", result.Count);
        return result;
    }

    public async Task AddPuntoONGAsync(PuntoONG punto)
    {
        _logger.LogInformation("▶ DonacionRepository.AddPuntoONGAsync({Nombre})", punto.Nombre);
        await _inner.AddPuntoONGAsync(punto);
        _logger.LogInformation("◀ DonacionRepository.AddPuntoONGAsync({Nombre}) → OK", punto.Nombre);
    }

    public async Task AddLoteAsync(LoteDonacion lote)
    {
        _logger.LogInformation("▶ DonacionRepository.AddLoteAsync({Id})", lote.Id);
        await _inner.AddLoteAsync(lote);
        _logger.LogInformation("◀ DonacionRepository.AddLoteAsync({Id}) → OK", lote.Id);
    }

    public async Task UpdateLoteAsync(LoteDonacion lote)
    {
        _logger.LogInformation("▶ DonacionRepository.UpdateLoteAsync({Id})", lote.Id);
        await _inner.UpdateLoteAsync(lote);
        _logger.LogInformation("◀ DonacionRepository.UpdateLoteAsync({Id}) → OK", lote.Id);
    }

    public async Task DeleteLoteAsync(int id)
    {
        _logger.LogInformation("▶ DonacionRepository.DeleteLoteAsync({Id})", id);
        await _inner.DeleteLoteAsync(id);
        _logger.LogInformation("◀ DonacionRepository.DeleteLoteAsync({Id}) → OK", id);
    }

    public async Task RemovePrendaFromLoteAsync(int loteId, int prendaId)
    {
        _logger.LogInformation("▶ DonacionRepository.RemovePrendaFromLoteAsync({LoteId}, {PrendaId})", loteId, prendaId);
        await _inner.RemovePrendaFromLoteAsync(loteId, prendaId);
        _logger.LogInformation("◀ DonacionRepository.RemovePrendaFromLoteAsync({LoteId}, {PrendaId}) → OK", loteId, prendaId);
    }
}
