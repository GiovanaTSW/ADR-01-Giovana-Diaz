namespace Dressly_MVC.ViewModels;

public class EstadisticasViewModel
{
    public int TotalPrendas { get; set; }
    public int TotalOutfits { get; set; }
    public int PrendasDonar { get; set; }
    public int EnDesuso { get; set; }

    public List<PrendaResumen> TopPrendas { get; set; } = new();
    public List<ConteoColor> DistribucionColores { get; set; } = new();
    public List<ConteoCategoria> DistribucionCategorias { get; set; } = new();
    public List<ConteoTemporada> DistribucionTemporadas { get; set; } = new();
}

public class PrendaResumen
{
    public string Nombre { get; set; } = string.Empty;
    public int VecesUsada { get; set; }
    public string? FotoUrl { get; set; }
}

public class ConteoColor
{
    public string Color { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public string HexCode { get; set; } = "#ccc";
}

public class ConteoCategoria
{
    public string Categoria { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}

public class ConteoTemporada
{
    public string Temporada { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}
