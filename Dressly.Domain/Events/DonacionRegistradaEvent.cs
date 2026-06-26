namespace Dressly.Domain.Events;

public record DonacionRegistradaEvent(int UsuarioId, int LoteId, int CantidadPrendas, DateTime Fecha);
