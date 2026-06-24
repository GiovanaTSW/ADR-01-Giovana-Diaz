namespace Dressly_MVC.Models;

public class Outfit
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Ocasion { get; set; } = string.Empty;
    public List<int> PrendaIds { get; set; } = new();
    public string Descripcion { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
}