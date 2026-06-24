using Dressly_MVC.Models;
using Dressly_MVC.Repositories;

namespace Dressly_MVC.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarios;

    public UsuarioService(IUsuarioRepository usuarios)
    {
        _usuarios = usuarios;
    }

    public Task<Usuario?> GetByIdAsync(int id)
        => _usuarios.GetByIdAsync(id);
}
