namespace Dressly_MVC.Services;

public interface IFotoService
{
    Task<string> GuardarAsync(IFormFile foto);
    Task EliminarAsync(string fotoUrl);
}
