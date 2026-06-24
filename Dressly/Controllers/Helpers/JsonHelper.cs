using System.Text.Json;

namespace Dressly_MVC.Helpers;

public static class JsonHelper
{
    private static string Ruta(string archivo) =>
        Path.Combine(Directory.GetCurrentDirectory(), "data", archivo);

    public static T? Leer<T>(string archivo)
    {
        var ruta = Ruta(archivo);
        if (!File.Exists(ruta)) return default;
        var json = File.ReadAllText(ruta);
        return JsonSerializer.Deserialize<T>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public static void Escribir<T>(string archivo, T datos)
    {
        var ruta = Ruta(archivo);
        Directory.CreateDirectory(Path.GetDirectoryName(ruta)!);
        var json = JsonSerializer.Serialize(datos,
            new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ruta, json);
    }
}