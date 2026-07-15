namespace Dressly.Domain.DomainServices;

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

    public static readonly HashSet<string> ColoresNeutros = new(StringComparer.OrdinalIgnoreCase)
    {
        "negro", "blanco", "gris", "beige", "crema", "gris perla", "blanco roto"
    };

    private static readonly Dictionary<string, double> HuePorColor;

    static ColorimetriaService()
    {
        HuePorColor = Paletas
            .SelectMany(p => p.Value)
            .GroupBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => HexToHue(g.First().Value), StringComparer.OrdinalIgnoreCase);
    }

    private static double HexToHue(string hex)
    {
        hex = hex.TrimStart('#');
        var r = Convert.ToInt32(hex[..2], 16) / 255.0;
        var g = Convert.ToInt32(hex[2..4], 16) / 255.0;
        var b = Convert.ToInt32(hex[4..6], 16) / 255.0;

        var max = Math.Max(r, Math.Max(g, b));
        var min = Math.Min(r, Math.Min(g, b));
        var delta = max - min;

        if (delta == 0) return 0;

        double hue;
        if (max == r)
            hue = 60 * (((g - b) / delta) % 6);
        else if (max == g)
            hue = 60 * (((b - r) / delta) + 2);
        else
            hue = 60 * (((r - g) / delta) + 4);

        if (hue < 0) hue += 360;
        return hue;
    }

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
        if (color1.Equals(color2, StringComparison.OrdinalIgnoreCase))
            return true;

        if (HuePorColor.TryGetValue(color1, out var h1) && HuePorColor.TryGetValue(color2, out var h2))
        {
            var diff = Math.Abs(h1 - h2);
            diff = Math.Min(diff, 360 - diff);
            return (diff >= 105 && diff <= 135) || (diff >= 225 && diff <= 255);
        }

        return false;
    }
}
