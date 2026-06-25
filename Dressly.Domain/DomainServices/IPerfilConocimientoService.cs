using Dressly.Domain.Entities;

namespace Dressly.Domain.DomainServices;

public interface IPerfilConocimientoService
{
    TipoCuerpoInfo? ObtenerInfoTipoCuerpo(string? tipoCuerpo);
    ColorimetriaInfo? ObtenerInfoColorimetria(string? colorimetria);
    ContrasteInfo? ObtenerInfoContraste(string? contraste);
    string? DetectarEstacion(string? subtonoPiel, string? intensidadCabello, string? colorOjos);
}
