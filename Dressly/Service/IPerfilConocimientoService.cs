using Dressly_MVC.Models;

namespace Dressly_MVC.Services;

public interface IPerfilConocimientoService
{
    TipoCuerpoInfo? ObtenerInfoTipoCuerpo(string? tipoCuerpo);
    ColorimetriaInfo? ObtenerInfoColorimetria(string? colorimetria);
    ContrasteInfo? ObtenerInfoContraste(string? contraste);
    string? DetectarEstacion(string? subtonoPiel, string? intensidadCabello, string? colorOjos);
}
