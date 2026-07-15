using Dressly.Domain.Entities;
using Dressly.Application.Ports.Input;
using Dressly.Application.Ports.Output;

namespace Dressly.Application.UseCases;

public class PerfilService : IPerfilService
{
    private readonly IUsuarioRepository _usuarios;

    public PerfilService(IUsuarioRepository usuarios)
    {
        _usuarios = usuarios;
    }

    public async Task<PerfilFisico?> GetPerfilAsync(int usuarioId)
    {
        var usuario = await _usuarios.GetByIdAsync(usuarioId);
        return usuario?.Perfil;
    }

    public async Task GuardarPerfilAsync(int usuarioId, PerfilFisico perfil)
    {
        perfil.UsuarioId = usuarioId;
        var usuario = await _usuarios.GetByIdAsync(usuarioId);
        if (usuario != null)
        {
            usuario.Perfil = perfil;
            await _usuarios.UpdateAsync(usuario);
        }
    }
    public async Task<IdentidadKibbeInfo?> ObtenerInfoKibbeAsync(int id)
    {
        // Por ahora, para que compile de inmediato y puedas arrancar el proyecto:
        return await Task.FromResult<IdentidadKibbeInfo?>(null);
    }
}
