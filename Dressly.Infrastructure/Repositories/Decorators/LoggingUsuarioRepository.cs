using Dressly.Application.Ports.Output;
using Dressly.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Dressly.Infrastructure.Repositories.Decorators;

public class LoggingUsuarioRepository : IUsuarioRepository
{
    private readonly IUsuarioRepository _inner;
    private readonly ILogger<LoggingUsuarioRepository> _logger;

    public LoggingUsuarioRepository(IUsuarioRepository inner, ILogger<LoggingUsuarioRepository> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<List<Usuario>> GetAllAsync()
    {
        _logger.LogInformation("▶ UsuarioRepository.GetAllAsync()");
        var result = await _inner.GetAllAsync();
        _logger.LogInformation("◀ UsuarioRepository.GetAllAsync() → {Count} items", result.Count);
        return result;
    }

    public async Task<Usuario?> GetByIdAsync(int id)
    {
        _logger.LogInformation("▶ UsuarioRepository.GetByIdAsync({Id})", id);
        var result = await _inner.GetByIdAsync(id);
        _logger.LogInformation("◀ UsuarioRepository.GetByIdAsync({Id}) → {Found}", id, result != null ? "encontrado" : "null");
        return result;
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        _logger.LogInformation("▶ UsuarioRepository.GetByEmailAsync({Email})", email);
        var result = await _inner.GetByEmailAsync(email);
        _logger.LogInformation("◀ UsuarioRepository.GetByEmailAsync({Email}) → {Found}", email, result != null ? "encontrado" : "null");
        return result;
    }

    public async Task<int> GetNextIdAsync()
    {
        _logger.LogInformation("▶ UsuarioRepository.GetNextIdAsync()");
        var result = await _inner.GetNextIdAsync();
        _logger.LogInformation("◀ UsuarioRepository.GetNextIdAsync() → {Id}", result);
        return result;
    }

    public async Task AddAsync(Usuario usuario)
    {
        _logger.LogInformation("▶ UsuarioRepository.AddAsync({Email})", usuario.Email);
        await _inner.AddAsync(usuario);
        _logger.LogInformation("◀ UsuarioRepository.AddAsync({Email}) → OK", usuario.Email);
    }

    public async Task UpdateAsync(Usuario usuario)
    {
        _logger.LogInformation("▶ UsuarioRepository.UpdateAsync({Id})", usuario.Id);
        await _inner.UpdateAsync(usuario);
        _logger.LogInformation("◀ UsuarioRepository.UpdateAsync({Id}) → OK", usuario.Id);
    }
}
