using Dressly_MVC.Models;
using Dressly_MVC.Repositories;

namespace Dressly_MVC.Services;

public class OutfitService : IOutfitService
{
    private readonly IOutfitRepository _outfits;
    private readonly IPrendaRepository _prendas;
    private readonly IColorimetriaService _colorimetria;
    private readonly IUsuarioRepository _usuarios;
    private readonly IPerfilService _perfil;
    private readonly IPerfilConocimientoService _conocimiento;

    public OutfitService(
        IOutfitRepository outfits,
        IPrendaRepository prendas,
        IColorimetriaService colorimetria,
        IUsuarioRepository usuarios,
        IPerfilService perfil,
        IPerfilConocimientoService conocimiento)
    {
        _outfits = outfits;
        _prendas = prendas;
        _colorimetria = colorimetria;
        _usuarios = usuarios;
        _perfil = perfil;
        _conocimiento = conocimiento;
    }

    public Task<List<Outfit>> GetOutfitsAsync(int usuarioId)
        => _outfits.GetByUsuarioIdAsync(usuarioId);

    public Task<Outfit?> GetByIdAsync(int id)
        => _outfits.GetByIdAsync(id);

    public async Task<List<Prenda>> GenerarSugerenciaAsync(int usuarioId, string ocasion)
    {
        var disponibles = await _prendas.GetDisponiblesAsync(usuarioId);
        var perfil = await _perfil.GetPerfilAsync(usuarioId);
        var colorInfo = _conocimiento.ObtenerInfoColorimetria(perfil?.Colorimetria);

        var coloresPaleta = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (colorInfo != null)
        {
            foreach (var c in colorInfo.ColoresPrincipales) coloresPaleta.Add(c.Key);
            foreach (var c in colorInfo.ColoresComplementarios) coloresPaleta.Add(c.Key);
            foreach (var c in colorInfo.ColoresNeutros) coloresPaleta.Add(c.Key);
        }

        // IDs de prendas ya usadas en outfits guardados → penalización
        var outfits = await _outfits.GetByUsuarioIdAsync(usuarioId);
        var idsEnOutfits = outfits.SelectMany(o => o.PrendaIds).ToHashSet();

        var prioridad = ocasion?.ToLower() switch
        {
            "formal" => new[] { "Superior", "Inferior", "Calzado", "Accesorio", "Vestido" },
            "deportivo" => new[] { "Calzado", "Inferior", "Superior", "Accesorio" },
            "playa" => new[] { "Superior", "Inferior", "Calzado", "Accesorio", "Vestido" },
            "fiesta" => new[] { "Vestido", "Calzado", "Accesorio" },
            "trabajo" => new[] { "Superior", "Inferior", "Calzado", "Accesorio" },
            _ => new[] { "Superior", "Inferior", "Calzado", "Accesorio", "Vestido" }
        };

        var sugerencia = new List<Prenda>();
        var rng = new Random();
        var categoriasSaltar = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var cat in prioridad)
        {
            if (categoriasSaltar.Contains(cat)) continue;

            var candidatas = disponibles
                .Where(p => p.Categoria == cat)
                .Select(p => new
                {
                    Prenda = p,
                    Score = Math.Max(CalcularPuntaje(p, sugerencia, coloresPaleta)
                        - (idsEnOutfits.Contains(p.Id) ? 20 : 0), 1)
                })
                .ToList();

            if (candidatas.Count == 0) continue;

            // Filtrar solo prendas compatibles con la última seleccionada
            List<(Prenda Prenda, int Score)> elegibles;
            if (sugerencia.Count > 0)
            {
                var ultima = sugerencia.Last();
                elegibles = candidatas
                    .Where(x => _colorimetria.SonCompatibles(ultima.Color, x.Prenda.Color))
                    .Select(x => (x.Prenda, x.Score))
                    .ToList();
            }
            else
            {
                elegibles = candidatas
                    .Select(x => (x.Prenda, x.Score))
                    .ToList();
            }

            // Fallback: si ninguna prenda compatible, usar todas las candidatas
            if (elegibles.Count == 0)
            {
                elegibles = candidatas
                    .Select(x => (x.Prenda, x.Score))
                    .ToList();
            }

            // Selección aleatoria entre el top 3 por puntaje
            var topN = elegibles
                .OrderByDescending(x => x.Score)
                .Take(3)
                .ToList();

            var seleccionada = topN[rng.Next(topN.Count)].Prenda;
            sugerencia.Add(seleccionada);

            if (seleccionada.Categoria == "Vestido")
            {
                categoriasSaltar.Add("Superior");
                categoriasSaltar.Add("Inferior");
            }
        }

        if (sugerencia.Count == 0 && disponibles.Count > 0)
        {
            sugerencia.Add(disponibles[rng.Next(disponibles.Count)]);
        }

        if (sugerencia.Any(p => p.Categoria == "Vestido"))
        {
            sugerencia.RemoveAll(p => p.Categoria == "Superior" || p.Categoria == "Inferior");
            var vestido = sugerencia.First(p => p.Categoria == "Vestido");
            sugerencia.Remove(vestido);
            sugerencia.Insert(0, vestido);
        }

        return sugerencia;
    }

    private int CalcularPuntaje(Prenda p, List<Prenda> seleccionadas, HashSet<string> coloresPaleta)
    {
        int score = 50;

        if (coloresPaleta.Contains(p.Color))
            score += 15;

        if (seleccionadas.Any())
        {
            var ultima = seleccionadas.Last();
            if (_colorimetria.SonCompatibles(ultima.Color, p.Color))
                score += 15;
        }

        var mes = DateTime.Now.Month;
        var estacionActual = mes switch
        {
            3 or 4 or 5 => "Primavera",
            6 or 7 or 8 => "Verano",
            9 or 10 or 11 => "Otoño",
            _ => "Invierno"
        };
        if (p.Estacion == "Todo el año" || p.Estacion == estacionActual)
            score += 10;

        if (p.VecesUsada <= 2)
            score += 5;

        return score;
    }

    public async Task<Outfit> GuardarOutfitAsync(int usuarioId, string nombre, string ocasion, List<int> prendaIds)
    {
        var todas = await _prendas.GetAllAsync();
        var descripcion = string.Join(", ", prendaIds
            .Select(id => todas.FirstOrDefault(p => p.Id == id)?.Nombre ?? $"#{id}"));

        var outfit = new Outfit
        {
            Nombre = nombre,
            Ocasion = ocasion,
            PrendaIds = prendaIds,
            Descripcion = descripcion,
            UsuarioId = usuarioId,
            FechaCreacion = DateTime.Now
        };

        await _outfits.AddAsync(outfit);
        return outfit;
    }

    public async Task<List<Prenda>> GetPrendasByOutfitAsync(Outfit outfit)
    {
        var todas = await _prendas.GetAllAsync();
        return outfit.PrendaIds
            .Select(id => todas.FirstOrDefault(p => p.Id == id))
            .Where(p => p != null)
            .Cast<Prenda>()
            .ToList();
    }
}
