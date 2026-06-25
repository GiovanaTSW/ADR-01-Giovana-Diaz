namespace Dressly.Domain.Entities;

public class TipoCuerpoInfo
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public List<string> Caracteristicas { get; set; } = new();
    public List<string> PrendasRecomendadas { get; set; } = new();
    public List<string> PrendasEvitar { get; set; } = new();
}
