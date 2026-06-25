namespace Dressly.Application.Ports.Input;

public interface ISeedService
{
    Task SeedUserDataAsync(int usuarioId);
}
