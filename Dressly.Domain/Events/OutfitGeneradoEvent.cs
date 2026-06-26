namespace Dressly.Domain.Events;

public record OutfitGeneradoEvent(int UsuarioId, int OutfitId, string NombreOutfit, DateTime Fecha);
