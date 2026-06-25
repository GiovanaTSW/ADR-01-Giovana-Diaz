using Dressly.Domain.Entities;
using Dressly.Application.Ports.Input;
using Dressly.Application.Ports.Output;

namespace Dressly.Application.UseCases;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IPerfilService _perfil;

    public UsuarioService(IUsuarioRepository usuarios, IPerfilService perfil)
    {
        _usuarios = usuarios;
        _perfil = perfil;
    }

    public async Task<Usuario?> GetByIdAsync(int id)
        => await _usuarios.GetByIdAsync(id);

    public async Task ActualizarPerfilAsync(int usuarioId, PerfilFisico perfil)
        => await _perfil.GuardarPerfilAsync(usuarioId, perfil);
}
