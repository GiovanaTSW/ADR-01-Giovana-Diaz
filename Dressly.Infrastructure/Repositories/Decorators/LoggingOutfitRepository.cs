using Dressly.Application.Ports.Output;
using Dressly.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Dressly.Infrastructure.Repositories.Decorators;

public class LoggingOutfitRepository : IOutfitRepository
{
    private readonly IOutfitRepository _inner;
    private readonly ILogger<LoggingOutfitRepository> _logger;

    public LoggingOutfitRepository(IOutfitRepository inner, ILogger<LoggingOutfitRepository> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<List<Outfit>> GetAllAsync()
    {
        _logger.LogInformation("▶ OutfitRepository.GetAllAsync()");
        var result = await _inner.GetAllAsync();
        _logger.LogInformation("◀ OutfitRepository.GetAllAsync() → {Count} items", result.Count);
        return result;
    }

    public async Task<Outfit?> GetByIdAsync(int id)
    {
        _logger.LogInformation("▶ OutfitRepository.GetByIdAsync({Id})", id);
        var result = await _inner.GetByIdAsync(id);
        _logger.LogInformation("◀ OutfitRepository.GetByIdAsync({Id}) → {Found}", id, result != null ? "encontrado" : "null");
        return result;
    }

    public async Task<List<Outfit>> GetByUsuarioIdAsync(int usuarioId)
    {
        _logger.LogInformation("▶ OutfitRepository.GetByUsuarioIdAsync({UsuarioId})", usuarioId);
        var result = await _inner.GetByUsuarioIdAsync(usuarioId);
        _logger.LogInformation("◀ OutfitRepository.GetByUsuarioIdAsync({UsuarioId}) → {Count} items", usuarioId, result.Count);
        return result;
    }

    public async Task<int> GetNextIdAsync()
    {
        _logger.LogInformation("▶ OutfitRepository.GetNextIdAsync()");
        var result = await _inner.GetNextIdAsync();
        _logger.LogInformation("◀ OutfitRepository.GetNextIdAsync() → {Id}", result);
        return result;
    }

    public async Task AddAsync(Outfit outfit)
    {
        _logger.LogInformation("▶ OutfitRepository.AddAsync({Nombre})", outfit.Nombre);
        await _inner.AddAsync(outfit);
        _logger.LogInformation("◀ OutfitRepository.AddAsync({Nombre}) → OK", outfit.Nombre);
    }

    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation("▶ OutfitRepository.DeleteAsync({Id})", id);
        await _inner.DeleteAsync(id);
        _logger.LogInformation("◀ OutfitRepository.DeleteAsync({Id}) → OK", id);
    }
}
