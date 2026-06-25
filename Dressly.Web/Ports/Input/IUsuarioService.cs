using Dressly.Domain.Entities;

namespace Dressly.Application.Ports.Input;

public interface IUsuarioService
{
    Task<Usuario?> GetByIdAsync(int id);
    Task ActualizarPerfilAsync(int usuarioId, PerfilFisico perfil);
}
