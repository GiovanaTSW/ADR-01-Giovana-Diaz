namespace Dressly.Domain.DomainServices;

public interface IColorimetriaService
{
    Dictionary<string, string> ObtenerColoresRecomendados(string? colorimetria);
    bool SonCompatibles(string color1, string color2);
}
