using Dressly.Domain.Entities;
using Dressly.Application.Ports.Input;
using Dressly.Application.Ports.Output;

namespace Dressly.Application.UseCases;

public class PerfilService : IPerfilService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IIdentidadKibbeRepository _kibbeRepo;

    public PerfilService(IUsuarioRepository usuarios, IIdentidadKibbeRepository kibbeRepo)
    {
        _usuarios = usuarios;
        _kibbeRepo = kibbeRepo;
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
            if (usuario.Perfil != null)
            {
                usuario.Perfil.TipoCuerpo = perfil.TipoCuerpo;
                usuario.Perfil.TonoPiel = perfil.TonoPiel;
                usuario.Perfil.SubtonoPiel = perfil.SubtonoPiel;
                usuario.Perfil.IntensidadCabello = perfil.IntensidadCabello;
                usuario.Perfil.ColorOjos = perfil.ColorOjos;
                usuario.Perfil.Colorimetria = perfil.Colorimetria;
                usuario.Perfil.Contraste = perfil.Contraste;
                usuario.Perfil.Altura = perfil.Altura;
                usuario.Perfil.KibbeInfoId = perfil.KibbeInfoId;
                usuario.Perfil.FotoUrl = perfil.FotoUrl;
                usuario.Perfil.Saturacion = perfil.Saturacion;
            }
            else
            {
                usuario.Perfil = perfil;
            }
            await _usuarios.UpdateAsync(usuario);
        }
    }
    public async Task<IdentidadKibbeInfo?> ObtenerInfoKibbeAsync(int id)
    {
        // Por ahora, para que compile de inmediato y puedas arrancar el proyecto:
        return await _kibbeRepo.GetByIdAsync(id);
    }
}
