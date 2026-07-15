namespace Dressly.Domain.DomainServices;

public static class ColorMapper
{
    public static bool EsNeutro(string color)
        => ColorimetriaService.ColoresNeutros.Contains(color);
}
