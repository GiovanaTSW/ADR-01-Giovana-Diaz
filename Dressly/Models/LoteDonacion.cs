namespace Dressly_MVC.Models;

public class LoteDonacion
{
    public int Id { get; set; }
    public List<int> PrendaIds { get; set; } = new();
    public int PuntoONGId { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public int UsuarioId { get; set; }
}