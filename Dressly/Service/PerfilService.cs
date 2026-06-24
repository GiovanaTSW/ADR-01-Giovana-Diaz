using Dressly_MVC.Models;
using Dressly_MVC.Repositories;

namespace Dressly_MVC.Services;

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

    public async Task GuardarPerfilAsync(int usuarioId, PerfilFisico perfil, PerfilFisico? existente = null)
    {
        var usuario = await _usuarios.GetByIdAsync(usuarioId);
        if (usuario != null)
        {
            perfil.UsuarioId = usuarioId;
            perfil.Id = existente?.Id ?? usuario.Perfil?.Id ?? usuarioId;
            usuario.Perfil = perfil;
            await _usuarios.UpdateAsync(usuario);
        }
    }
}
