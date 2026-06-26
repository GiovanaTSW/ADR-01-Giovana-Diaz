namespace Dressly.Domain.Events;

public record PrendaCreadaEvent(int UsuarioId, int PrendaId, string NombrePrenda, DateTime Fecha);
