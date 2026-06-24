using Dressly_MVC.Models;

namespace Dressly_MVC.Services;

public interface IPerfilService
{
    Task<PerfilFisico?> GetPerfilAsync(int usuarioId);
    Task GuardarPerfilAsync(int usuarioId, PerfilFisico perfil, PerfilFisico? existente = null);
}
