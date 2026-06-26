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
        _logger.LogInformation("UsuarioRepository.GetAllAsync - inicio");
        var result = await _inner.GetAllAsync();
        _logger.LogInformation("UsuarioRepository.GetAllAsync - {Count} items", result.Count);
        return result;
    }

    public async Task<Usuario?> GetByIdAsync(int id)
    {
        _logger.LogInformation("UsuarioRepository.GetByIdAsync({Id}) - inicio", id);
        var result = await _inner.GetByIdAsync(id);
        _logger.LogInformation("UsuarioRepository.GetByIdAsync({Id}) - {Status}", id, result != null ? "encontrado" : "null");
        return result;
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        _logger.LogInformation("UsuarioRepository.GetByEmailAsync({Email}) - inicio", email);
        var result = await _inner.GetByEmailAsync(email);
        _logger.LogInformation("UsuarioRepository.GetByEmailAsync({Email}) - {Status}", email, result != null ? "encontrado" : "null");
        return result;
    }

    public async Task<int> GetNextIdAsync()
    {
        _logger.LogInformation("UsuarioRepository.GetNextIdAsync - inicio");
        var result = await _inner.GetNextIdAsync();
        _logger.LogInformation("UsuarioRepository.GetNextIdAsync - id {Id}", result);
        return result;
    }

    public async Task AddAsync(Usuario usuario)
    {
        _logger.LogInformation("UsuarioRepository.AddAsync({Email}) - inicio", usuario.Email);
        await _inner.AddAsync(usuario);
        _logger.LogInformation("UsuarioRepository.AddAsync({Email}) - guardado", usuario.Email);
    }

    public async Task UpdateAsync(Usuario usuario)
    {
        _logger.LogInformation("UsuarioRepository.UpdateAsync({Id}) - inicio", usuario.Id);
        await _inner.UpdateAsync(usuario);
        _logger.LogInformation("UsuarioRepository.UpdateAsync({Id}) - actualizado", usuario.Id);
    }
}
