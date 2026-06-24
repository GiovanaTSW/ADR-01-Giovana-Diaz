namespace Dressly_MVC.Models;

public class DonacionData
{
    public List<LoteDonacion> Lotes { get; set; } = new();
    public List<PuntoONG> PuntosONG { get; set; } = new();
}

public class PuntoONG
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public double Latitud { get; set; }
    public double Longitud { get; set; }
    public string Telefono { get; set; } = string.Empty;
}