namespace Dressly_MVC.Services;

public class FotoService : IFotoService
{
    private readonly string _carpeta;

    public FotoService()
    {
        _carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        Directory.CreateDirectory(_carpeta);
    }

    public async Task<string> GuardarAsync(IFormFile foto)
    {
        var nombreUnico = $"{Guid.NewGuid()}_{foto.FileName}";
        var ruta = Path.Combine(_carpeta, nombreUnico);
        using var stream = new FileStream(ruta, FileMode.Create);
        await foto.CopyToAsync(stream);
        return $"/uploads/{nombreUnico}";
    }

    public Task EliminarAsync(string fotoUrl)
    {
        if (string.IsNullOrEmpty(fotoUrl)) return Task.CompletedTask;
        var nombre = Path.GetFileName(fotoUrl);
        var ruta = Path.Combine(_carpeta, nombre);
        if (File.Exists(ruta)) File.Delete(ruta);
        return Task.CompletedTask;
    }
}
