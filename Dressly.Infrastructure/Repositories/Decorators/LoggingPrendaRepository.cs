using Dressly.Application.Ports.Output;
using Dressly.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Dressly.Infrastructure.Repositories.Decorators;

public class LoggingPrendaRepository : IPrendaRepository
{
    private readonly IPrendaRepository _inner;
    private readonly ILogger<LoggingPrendaRepository> _logger;

    public LoggingPrendaRepository(IPrendaRepository inner, ILogger<LoggingPrendaRepository> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<List<Prenda>> GetAllAsync()
    {
        _logger.LogInformation("PrendaRepository.GetAllAsync - inicio");
        var result = await _inner.GetAllAsync();
        _logger.LogInformation("PrendaRepository.GetAllAsync - {Count} items", result.Count);
        return result;
    }

    public async Task<Prenda?> GetByIdAsync(int id)
    {
        _logger.LogInformation("PrendaRepository.GetByIdAsync({Id}) - inicio", id);
        var result = await _inner.GetByIdAsync(id);
        _logger.LogInformation("PrendaRepository.GetByIdAsync({Id}) - {Status}", id, result != null ? "encontrado" : "null");
        return result;
    }

    public async Task<List<Prenda>> GetByUsuarioIdAsync(int usuarioId)
    {
        _logger.LogInformation("PrendaRepository.GetByUsuarioIdAsync({UsuarioId}) - inicio", usuarioId);
        var result = await _inner.GetByUsuarioIdAsync(usuarioId);
        _logger.LogInformation("PrendaRepository.GetByUsuarioIdAsync({UsuarioId}) - {Count} items", usuarioId, result.Count);
        return result;
    }

    public async Task<List<Prenda>> GetDisponiblesAsync(int usuarioId)
    {
        _logger.LogInformation("PrendaRepository.GetDisponiblesAsync({UsuarioId}) - inicio", usuarioId);
        var result = await _inner.GetDisponiblesAsync(usuarioId);
        _logger.LogInformation("PrendaRepository.GetDisponiblesAsync({UsuarioId}) - {Count} items", usuarioId, result.Count);
        return result;
    }

    public async Task<List<Prenda>> GetDisponiblesParaDonarAsync(int usuarioId)
    {
        _logger.LogInformation("PrendaRepository.GetDisponiblesParaDonarAsync({UsuarioId}) - inicio", usuarioId);
        var result = await _inner.GetDisponiblesParaDonarAsync(usuarioId);
        _logger.LogInformation("PrendaRepository.GetDisponiblesParaDonarAsync({UsuarioId}) - {Count} items", usuarioId, result.Count);
        return result;
    }

    public async Task<int> GetNextIdAsync()
    {
        _logger.LogInformation("PrendaRepository.GetNextIdAsync - inicio");
        var result = await _inner.GetNextIdAsync();
        _logger.LogInformation("PrendaRepository.GetNextIdAsync - id {Id}", result);
        return result;
    }

    public async Task AddAsync(Prenda prenda)
    {
        _logger.LogInformation("PrendaRepository.AddAsync({Nombre}) - inicio", prenda.Nombre);
        await _inner.AddAsync(prenda);
        _logger.LogInformation("PrendaRepository.AddAsync({Nombre}) - guardado", prenda.Nombre);
    }

    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation("PrendaRepository.DeleteAsync({Id}) - inicio", id);
        await _inner.DeleteAsync(id);
        _logger.LogInformation("PrendaRepository.DeleteAsync({Id}) - eliminado", id);
    }

    public async Task SaveAsync(List<Prenda> prendas)
    {
        _logger.LogInformation("PrendaRepository.SaveAsync({Count} items) - inicio", prendas.Count);
        await _inner.SaveAsync(prendas);
        _logger.LogInformation("PrendaRepository.SaveAsync - guardado");
    }
}
