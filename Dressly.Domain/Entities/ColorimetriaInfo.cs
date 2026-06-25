namespace Dressly.Domain.Entities;

public class ColorimetriaInfo
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public List<string> Caracteristicas { get; set; } = new();
    public string Explicacion { get; set; } = string.Empty;
    public Dictionary<string, string> ColoresPrincipales { get; set; } = new();
    public Dictionary<string, string> ColoresComplementarios { get; set; } = new();
    public Dictionary<string, string> ColoresNeutros { get; set; } = new();
}
