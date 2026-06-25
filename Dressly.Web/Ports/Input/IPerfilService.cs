using Dressly.Domain.Entities;

namespace Dressly.Application.Ports.Input;

public interface IPerfilService
{
    Task<PerfilFisico?> GetPerfilAsync(int usuarioId);
    Task GuardarPerfilAsync(int usuarioId, PerfilFisico perfil);
}
