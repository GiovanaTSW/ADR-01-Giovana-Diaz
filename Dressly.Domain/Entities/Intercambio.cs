namespace Dressly.Domain.Entities;

public enum EstadoIntercambio
{
    Publicado,
    Propuesto,
    Aceptado,
    Rechazado,
    Completado
}

public class Intercambio
{
    public int Id { get; set; }
    public int UsuarioOfertanteId { get; set; }
    public int? UsuarioInteresadoId { get; set; }
    public int PrendaOfertadaId { get; set; }
    public int? PrendaInteresadoId { get; set; }
    public EstadoIntercambio Estado { get; set; } = EstadoIntercambio.Publicado;
    public decimal Comision { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
}
