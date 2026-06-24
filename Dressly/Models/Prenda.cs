namespace Dressly_MVC.Models;

public class Prenda
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Talla { get; set; } = string.Empty;
    public string Estacion { get; set; } = string.Empty;
    public string FotoUrl { get; set; } = string.Empty;
    public int VecesUsada { get; set; } = 0;
    public DateTime FechaUltimoUso { get; set; } = DateTime.Now;
    public bool EnDesuso { get; set; } = false;
    public int UsuarioId { get; set; }
}