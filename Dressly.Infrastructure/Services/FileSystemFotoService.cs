using Dressly.Application.Ports.Input;

namespace Dressly.Infrastructure.Services;

public class FileSystemFotoService : IAlmacenamientoImagenes
{
    private readonly string _carpeta;

    public FileSystemFotoService()
    {
        _carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        Directory.CreateDirectory(_carpeta);
    }

    public async Task<string> GuardarAsync(byte[] contenido, string nombreArchivo)
    {
        var nombreUnico = $"{Guid.NewGuid()}_{nombreArchivo}";
        var ruta = Path.Combine(_carpeta, nombreUnico);
        await File.WriteAllBytesAsync(ruta, contenido);
        return $"/uploads/{nombreUnico}";
    }

    public Task EliminarAsync(string urlImagen)
    {
        if (string.IsNullOrEmpty(urlImagen)) return Task.CompletedTask;
        var nombre = Path.GetFileName(urlImagen);
        var ruta = Path.Combine(_carpeta, nombre);
        if (File.Exists(ruta)) File.Delete(ruta);
        return Task.CompletedTask;
    }
}
