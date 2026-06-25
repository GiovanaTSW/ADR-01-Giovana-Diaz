namespace Dressly.Domain.Entities;

public class ContrasteInfo
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Explicacion { get; set; } = string.Empty;
    public List<string> Recomendaciones { get; set; } = new();
}
