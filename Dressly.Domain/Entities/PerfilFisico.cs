namespace Dressly.Domain.Entities;

public class PerfilFisico
{
    public int Id { get; set; }
    public string TipoCuerpo { get; set; } = string.Empty;
    public string TonoPiel { get; set; } = string.Empty;
    public string SubtonoPiel { get; set; } = string.Empty;
    public string IntensidadCabello { get; set; } = string.Empty;
    public string ColorOjos { get; set; } = string.Empty;
    public string Colorimetria { get; set; } = string.Empty;
    public string Contraste { get; set; } = string.Empty;
    public decimal? Altura { get; set; }
    public int UsuarioId { get; set; }
    public string? FotoUrl { get; set; }
}
