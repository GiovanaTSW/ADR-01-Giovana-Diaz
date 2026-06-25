namespace Dressly.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public PerfilFisico? Perfil { get; set; }
    public List<Prenda> Prendas { get; set; } = new();
}
