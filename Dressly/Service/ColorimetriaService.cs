namespace Dressly_MVC.Services;

public class ColorimetriaService : IColorimetriaService
{
    private static readonly Dictionary<string, Dictionary<string, string>> Paletas = new()
    {
        ["Primavera"] = new()
        {
            ["coral"] = "#FF7043", ["melocotón"] = "#FFAB91",
            ["beige"] = "#F5F5DC", ["dorado"] = "#FFC107",
            ["verde claro"] = "#A5D6A7"
        },
        ["Verano"] = new()
        {
            ["lavanda"] = "#CE93D8", ["azul grisáceo"] = "#90A4AE",
            ["rosa palo"] = "#F8BBD0", ["gris perla"] = "#ECEFF1",
            ["blanco roto"] = "#FAFAFA"
        },
        ["Otoño"] = new()
        {
            ["terracota"] = "#BF360C", ["mostaza"] = "#F9A825",
            ["verde oliva"] = "#827717", ["marrón"] = "#795548",
            ["naranja"] = "#E64A19"
        },
        ["Invierno"] = new()
        {
            ["negro"] = "#212121", ["blanco"] = "#FAFAFA",
            ["rojo"] = "#B71C1C", ["azul marino"] = "#0D47A1",
            ["fucsia"] = "#AD1457"
        }
    };

    private static readonly HashSet<string> ColoresNeutros = new(StringComparer.OrdinalIgnoreCase)
    {
        "negro", "blanco", "gris", "beige", "crema", "gris perla", "blanco roto"
    };

    public Dictionary<string, string> ObtenerColoresRecomendados(string? colorimetria)
    {
        if (!string.IsNullOrEmpty(colorimetria) && Paletas.ContainsKey(colorimetria))
            return Paletas[colorimetria];
        return Paletas["Invierno"];
    }

    public bool SonCompatibles(string color1, string color2)
    {
        if (string.IsNullOrEmpty(color1) || string.IsNullOrEmpty(color2))
            return true;
        if (ColoresNeutros.Contains(color1) || ColoresNeutros.Contains(color2))
            return true;
        return color1.Equals(color2, StringComparison.OrdinalIgnoreCase);
    }
}
