namespace Dressly.Domain.Entities;

public class Patrocinio
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public int PuntoONGId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public decimal Monto { get; set; }
    public bool Activo { get; set; } = true;
}
