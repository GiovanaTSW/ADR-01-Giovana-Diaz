using Dressly.Domain.Entities;

namespace Dressly_MVC.ViewModels;

public class PerfilEditarViewModel
{
    public PerfilFisico? Perfil { get; set; }
    public List<IdentidadKibbeInfo> Identidades { get; set; } = new();
    public Dictionary<string, string> ImagenesPorNombre { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}