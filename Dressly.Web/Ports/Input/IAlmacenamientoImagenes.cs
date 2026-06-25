namespace Dressly.Application.Ports.Input;

public interface IAlmacenamientoImagenes
{
    Task<string> GuardarAsync(byte[] contenido, string nombreArchivo);
    Task EliminarAsync(string urlImagen);
}
