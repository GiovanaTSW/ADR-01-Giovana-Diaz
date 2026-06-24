using Dressly_MVC.Models;

namespace Dressly_MVC.ViewModels;

public class PerfilViewModel
{
    public PerfilFisico? Perfil { get; set; }
    public TipoCuerpoInfo? TipoCuerpoInfo { get; set; }
    public ColorimetriaInfo? ColorimetriaInfo { get; set; }
    public ContrasteInfo? ContrasteInfo { get; set; }
}
