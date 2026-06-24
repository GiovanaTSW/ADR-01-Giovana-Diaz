namespace Dressly_MVC.Services;

public interface IColorimetriaService
{
    Dictionary<string, string> ObtenerColoresRecomendados(string? colorimetria);
    bool SonCompatibles(string color1, string color2);
}
